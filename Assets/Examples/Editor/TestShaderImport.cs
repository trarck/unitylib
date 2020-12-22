public class TestShaderImport{
    public static void GetShaderImporterTextureNames(ShaderImporter shaderImporter,ref List<string> names)
    {
        var so = new SerializedObject(shaderImporter);
        var defaultTextures = so.FindProperty("m_DefaultTextures");

        for (int i = defaultTextures.arraySize - 1; i >= 0; i--)
        {
            var propertyName = defaultTextures.GetArrayElementAtIndex(i).displayName;
            names.Add(propertyName);
        }
    }
}