using UnityEngine;

public static class GlobalHelper
{
    private static int objID;

    private static float moveUnit;
    public static int GenerateUniqueID(GameObject obj)
    {
        objID++;
        return objID;
    }
}
