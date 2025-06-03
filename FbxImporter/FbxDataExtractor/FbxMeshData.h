#pragma once

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Numerics;

namespace FbxDataExtractor
{
	public ref class FbxVertexData
	{
	internal:
		FbxVertexData() {}
	public:
		Vector3 Position;

		Vector3 Normal;

		Vector4 Bitangent;

		List<Vector4>^ Tangents;

		List<Vector2>^ UVs;

		List<Vector4>^ Colors;

		array<String^>^ BoneNames;

		array<float>^ BoneWeights;
	};

	public ref class FbxMeshData
	{
	public:
		String^ Name;

		List<int>^ VertexIndices;

		List<FbxVertexData^>^ VertexData;

		static List<FbxMeshData^>^ Import(String^ path);
	internal:
		FbxMeshData(const char* name);
	};
}
