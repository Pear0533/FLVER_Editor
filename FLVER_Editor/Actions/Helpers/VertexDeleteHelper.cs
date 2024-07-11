using SoulsFormats;

namespace FLVER_Editor.Actions.Helpers;

public static class VertexDeleteHelper
{
    public static void DeleteMeshVertexFaceSet(FLVER2.Mesh mesh, int index, Dictionary<FLVER2.FaceSet, Dictionary<int, int>> indexesBackup)
    {
        foreach (FLVER2.FaceSet faceSet in mesh.FaceSets)
        {
            Dictionary<int, int>? backup = null;
            if (!indexesBackup.TryGetValue(faceSet, out backup))
            {
                backup = new Dictionary<int, int>();
                indexesBackup.Add(faceSet, backup);
            }

            for (int i = 0; i + 2 < faceSet.Indices.Count; i += 3)
            {
                if (faceSet.Indices[i] == index || faceSet.Indices[i + 1] == index || faceSet.Indices[i + 2] == index)
                {
                    backup.Add(i, faceSet.Indices[i]);
                    faceSet.Indices[i] = index;

                    backup.Add(i + 1, faceSet.Indices[i + 1]);
                    faceSet.Indices[i + 1] = index;

                    backup.Add(i + 2, faceSet.Indices[i + 2]);
                    faceSet.Indices[i + 2] = index;
                }
            }
        }
    }

    public static void RestoreMeshVertexFaceSet(Dictionary<FLVER2.FaceSet, Dictionary<int, int>> indexesBackup)
    {
        foreach (var facebackup in indexesBackup)
        {
            var faceSet = facebackup.Key;
            var indices = facebackup.Value;

            foreach (var item in indices)
            {
                var index = item.Key;
                var value = item.Value;
                faceSet.Indices[index] = value;
            }
        }
    }
}