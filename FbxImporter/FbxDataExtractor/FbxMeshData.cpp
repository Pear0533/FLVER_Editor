#include "pch.h"
#include <msclr/marshal_cppstd.h>
#include <vector>
#include <future>
#include <unordered_map>
#include <ranges>

#include <fbxsdk.h>

#include "FbxMeshData.h"
#include "Util.h"

struct FbxPolygonVertex
{
	int Id;
	FbxVector4 Normal;
	FbxVector4 Bitangent;
	std::vector<FbxVector4> Tangents;
	std::vector<FbxVector2> UVs;
	std::vector<FbxColor> Colors;

	bool operator==(const FbxPolygonVertex& other) const
	{
		return Normal == other.Normal
			&& Bitangent == other.Bitangent
			&& FbxDataExtractor::SequenceEqual(Tangents, other.Tangents)
			&& FbxDataExtractor::SequenceEqual(UVs, other.UVs)
			&& FbxDataExtractor::SequenceEqual(Colors, other.Colors);
	}
};

struct FbxControlPoint
{
	FbxVector4 Position;
	std::vector<const char*> BoneNames;
	std::vector<double> BoneWeights;
	FbxPolygonVertex FinalVertex;
	std::vector<FbxPolygonVertex> PolygonVertices;
};

struct FbxExtractedMesh
{
	const char* Name;
	std::vector<FbxControlPoint> Vertices;
	std::vector<int> Triangles;
};

namespace std
{
	template <typename T, typename... Rest>
	void hash_combine(std::size_t& seed, const T& v, const Rest&... rest)
	{
		seed ^= std::hash<T> { }(v) + 0x9e3779b9 + (seed << 6) + (seed >> 2);
		(hash_combine(seed, rest), ...);
	}

	template <>
	struct hash<FbxPolygonVertex>
	{
		std::size_t operator()(FbxPolygonVertex const& vertex) const noexcept
		{
			std::size_t hash = 0;
			hash_combine(hash, vertex.Normal, vertex.Bitangent);
			if (vertex.Tangents.size() > 0) hash_combine(hash, vertex.Tangents[0]);
			if (vertex.UVs.size() > 0) hash_combine(hash, vertex.UVs[0]);
			if (vertex.Colors.size() > 0) hash_combine(hash, vertex.Colors[0]);
			return hash;
		}
	};

	template <>
	struct hash<FbxVector4>
	{
		std::size_t operator()(FbxVector4 const& vector) const noexcept
		{
			std::size_t hash = 0;
			hash_combine(hash, vector[0], vector[1], vector[2], vector[3]);
			return hash;
		}
	};

	template <>
	struct hash<FbxColor>
	{
		std::size_t operator()(FbxColor const& color) const noexcept
		{
			std::size_t hash = 0;
			hash_combine(hash, color[0], color[1], color[2], color[3]);
			return hash;
		}
	};

	template <>
	struct hash<FbxVector2>
	{
		std::size_t operator()(FbxVector2 const& vector) const noexcept
		{
			std::size_t hash = 0;
			hash_combine(hash, vector[0], vector[1]);
			return hash;
		}
	};
}

namespace FbxDataExtractor
{
	static std::mutex skinMutex;

	FbxMeshData::FbxMeshData(const char* name)
	{
		Name = gcnew String(name);
	}

	static void GetVertexData(const FbxMesh* fbxMesh, std::vector<FbxControlPoint>& controlPoints, std::vector<int>& vertexIndices)
	{
		controlPoints = std::vector<FbxControlPoint>(fbxMesh->GetControlPointsCount());

		const int avgPolygonVertexCount = controlPoints.size() > 0 ? fbxMesh->GetPolygonVertexCount() / (int)controlPoints.size() : 0;
		for (int i = 0; i < controlPoints.size(); i++)
		{
			FbxControlPoint& controlPoint = controlPoints.at(i);
			controlPoint.Position = fbxMesh->GetControlPointAt(i);
			controlPoint.PolygonVertices.reserve(avgPolygonVertexCount);
		}

		// getting element counts seems to have a performance impact so we cache them
		const int tangentCount = fbxMesh->GetElementTangentCount();
		const int uvCount = fbxMesh->GetElementUVCount();
		const int colorCount = fbxMesh->GetElementVertexColorCount();

		vertexIndices = std::vector<int>(fbxMesh->GetPolygonVertexCount());
		const int* controlPointIndices = fbxMesh->GetPolygonVertices();
		for (int i = 0; i < vertexIndices.size(); i++)
		{
			const int controlPointIndex = controlPointIndices[i];
			vertexIndices[i] = controlPointIndex;
			FbxPolygonVertex& polygonVertex = controlPoints.at(controlPointIndex).PolygonVertices.emplace_back();

			polygonVertex.Id = i;
			polygonVertex.Normal = GetLayerElementValue(*fbxMesh->GetElementNormal(), controlPointIndex, i);

			if (fbxMesh->GetElementBinormalCount() > 0)
			{
				polygonVertex.Bitangent = GetLayerElementValue(*fbxMesh->GetElementBinormal(), controlPointIndex, i);
			}

			polygonVertex.Tangents.reserve(tangentCount);
			for (int j = 0; j < tangentCount; j++)
			{
				polygonVertex.Tangents.emplace_back(GetLayerElementValue(*fbxMesh->GetElementTangent(j), controlPointIndex, i));
			}

			polygonVertex.UVs.reserve(uvCount);
			for (int j = 0; j < uvCount; j++)
			{
				polygonVertex.UVs.emplace_back(GetLayerElementValue(*fbxMesh->GetElementUV(j), controlPointIndex, i));
			}

			polygonVertex.Colors.reserve(colorCount);
			for (int j = 0; j < colorCount; j++)
			{
				polygonVertex.Colors.emplace_back(GetLayerElementValue(*fbxMesh->GetElementVertexColor(j), controlPointIndex, i));
			}
		}
	}

	static void GetSkinData(const FbxMesh* fbxMesh, std::vector<FbxControlPoint>& controlPoints)
	{
		const FbxSkin* skin = nullptr;
		for (int i = 0; i < fbxMesh->GetDeformerCount(); i++)
		{
			if (fbxMesh->GetDeformer(i)->GetDeformerType() == FbxDeformer::eSkin)
			{
				skin = static_cast<FbxSkin*>(fbxMesh->GetDeformer(i));
				break;
			}
		}

		if (!skin) return;

		for (int i = 0; i < skin->GetClusterCount(); i++)
		{
			const FbxCluster* cluster = skin->GetCluster(i);
			const int* clusterControlPointIndices = cluster->GetControlPointIndices();
			const double* controlPointWeights = cluster->GetControlPointWeights();
			for (int j = 0; j < cluster->GetControlPointIndicesCount(); j++)
			{
				const int ccpIndex = clusterControlPointIndices[j];
				FbxControlPoint& controlPoint = controlPoints.at(ccpIndex);
				controlPoint.BoneNames.emplace_back(cluster->GetLink()->GetName());
				controlPoint.BoneWeights.emplace_back(controlPointWeights[j]);
			}
		}
	}

	static void FlattenVertices(std::vector<FbxControlPoint>& controlPoints, std::vector<int>& vertexIndices)
	{
		std::vector vertexIndicesRemap(vertexIndices);
		const int controlPointCount = (int)controlPoints.size();
		for (int i = 0; i < controlPointCount; ++i)
		{
			const FbxControlPoint controlPoint = std::move(controlPoints.at(i));
			std::unordered_map<FbxPolygonVertex, std::vector<int>> vertexGroups { };
			for (int j = 0; j < controlPoint.PolygonVertices.size(); ++j)
			{
				const FbxPolygonVertex& polygonVertex = controlPoint.PolygonVertices[j];
				vertexGroups.try_emplace(polygonVertex, std::vector<int>());
				vertexGroups[polygonVertex].push_back(polygonVertex.Id);
			}

			auto current = vertexGroups.begin();
			int currentIndex = i;
			while (current != vertexGroups.end())
			{
				if (currentIndex != i) controlPoints.emplace_back();

				controlPoints.at(currentIndex) = FbxControlPoint {
					.Position = controlPoint.Position,
					.BoneNames = controlPoint.BoneNames,
					.BoneWeights = controlPoint.BoneWeights,
					.FinalVertex = current->first
				};

				for (const int polygonIndex : current->second)
				{
					vertexIndices.at(polygonIndex) = currentIndex;
				}

				currentIndex = (int)controlPoints.size();
				++current;
			}
		}
	}

	static List<FbxVertexData^>^ ToClrVertices(const std::vector<FbxControlPoint>& vertices)
	{
		List<FbxVertexData^>^ clrData = gcnew List<FbxVertexData^>((int)vertices.size());
		for (const FbxControlPoint& vertex : vertices)
		{
			FbxVertexData^ clrVertex = gcnew FbxVertexData();

			FbxVector4 position = vertex.Position;
			clrVertex->Position = FbxVector4ToVector3(position);

			clrVertex->BoneNames = gcnew array<String^>((int)vertex.BoneNames.size());
			for (int i = 0; i < (int)vertex.BoneNames.size(); ++i)
			{
				clrVertex->BoneNames[i] = gcnew String(vertex.BoneNames[i]);
			}

			clrVertex->BoneWeights = gcnew array<float>((int)vertex.BoneWeights.size());
			for (int i = 0; i < (int)vertex.BoneWeights.size(); ++i)
			{
				clrVertex->BoneWeights[i] = static_cast<float>(vertex.BoneWeights[i]);
			}

			FbxVector4 normal = vertex.FinalVertex.Normal;
			clrVertex->Normal = FbxVector4ToVector3(normal);

			FbxVector4 bitangent = vertex.FinalVertex.Bitangent;
			clrVertex->Bitangent = FbxVector4ToVector4(bitangent);

			clrVertex->Tangents = gcnew List<Vector4>((int)vertex.FinalVertex.Tangents.size());
			for (const FbxVector4& tangent : vertex.FinalVertex.Tangents)
			{
				clrVertex->Tangents->Add(FbxVector4ToVector4(tangent));
			}

			clrVertex->UVs = gcnew List<Vector2>((int)vertex.FinalVertex.UVs.size());
			for (const FbxVector2& uv : vertex.FinalVertex.UVs)
			{
				clrVertex->UVs->Add(FbxVector2ToVector2(uv));
			}

			clrVertex->Colors = gcnew List<Vector4>((int)vertex.FinalVertex.Colors.size());
			for (const FbxColor& color : vertex.FinalVertex.Colors)
			{
				clrVertex->Colors->Add(FbxColorToVector4(color));
			}

			clrData->Add(clrVertex);
		}
		return clrData;
	}

	FbxExtractedMesh ImportMesh(const FbxMesh* fbxMesh)
	{
		FbxExtractedMesh extractedMesh;
		extractedMesh.Name = fbxMesh->GetNode()->GetName();
		GetVertexData(fbxMesh, extractedMesh.Vertices, extractedMesh.Triangles);
		{
			// GetSkinData cannot be made thread safe as FbxCluster operations are not thread safe even if const
			std::lock_guard lock(skinMutex);
			GetSkinData(fbxMesh, extractedMesh.Vertices);
		}
		FlattenVertices(extractedMesh.Vertices, extractedMesh.Triangles);
		return extractedMesh;
	}

	List<FbxMeshData^>^ FbxMeshData::Import(String^ path)
	{
		FbxManager* fbxManager = FbxManager::Create();

		FbxIOSettings* ios = FbxIOSettings::Create(fbxManager, IOSROOT);
		fbxManager->SetIOSettings(ios);
		FbxImporter* importer = FbxImporter::Create(fbxManager, "");

		const std::string pathPtr = msclr::interop::marshal_as<std::string>(path);

		importer->Initialize(pathPtr.c_str(), -1, ios);

		FbxScene* scene = FbxScene::Create(fbxManager, "");
		importer->Import(scene);
		importer->Destroy();
		ios->Destroy();

		FbxGeometryConverter geometryConverter(fbxManager);
		geometryConverter.Triangulate(scene, true);

		std::vector<FbxMesh*> meshes;
		for (int i = 0; i < scene->GetNodeCount(); i++)
		{
			FbxNode* node = scene->GetNode(i);
			FbxMesh* mesh = node->GetMesh();
			if (!mesh) continue;

			mesh->GenerateNormals();
			mesh->GenerateTangentsDataForAllUVSets();
			meshes.push_back(mesh);
		}

		std::vector<std::future<FbxExtractedMesh>> tasks;
		for (FbxMesh* mesh : meshes)
		{
			tasks.emplace_back(std::async(std::launch::async, &ImportMesh, mesh));
		}

		List<FbxMeshData^>^ meshList = gcnew List<FbxMeshData^>();
		for (std::future<FbxExtractedMesh>& task : tasks)
		{
			FbxExtractedMesh extractedMesh = std::move(task.get());

			FbxMeshData^ clrMesh = gcnew FbxMeshData(extractedMesh.Name);
			clrMesh->VertexData = ToClrVertices(extractedMesh.Vertices);

			clrMesh->VertexIndices = gcnew List<int>((int)extractedMesh.Vertices.size());
			for (const int vertexIndex : extractedMesh.Triangles)
			{
				clrMesh->VertexIndices->Add(vertexIndex);
			}
			meshList->Add(clrMesh);
		}

		fbxManager->Destroy();

		return meshList;
	}
}
