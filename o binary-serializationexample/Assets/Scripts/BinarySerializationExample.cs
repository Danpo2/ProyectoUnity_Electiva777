using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class BinarySerializationExample : MonoBehaviour
{
    void Start()
    {
        var data = new MyData
        {
            shield = 100,
            health = 50,
            name = "Sven The Destroyer",
            position = new Vector3(1, 2, 3)
        };

        Debug.Log($"Original: {data}");
        byte[] bytes = ToBytes(data);
        MyData copy = ToObject<MyData>(bytes);
        Debug.Log($"Copy: {copy}");
    }

    private T ToObject<T>(byte[] data)
    {
        var size = Marshal.SizeOf(typeof(T));
        var ptr = Marshal.AllocHGlobal(size);
        Marshal.Copy(data, 0, ptr, size);

        var copyData = (T)Marshal.PtrToStructure(ptr, typeof(T));
        Marshal.FreeHGlobal(ptr);
        return copyData;
    }

    private byte[] ToBytes(object data)
    {
        var size = Marshal.SizeOf(data);
        byte[] buf = new byte[size];
        var ptr = Marshal.AllocHGlobal(size);

        Marshal.StructureToPtr(data, ptr, true);
        Marshal.Copy(ptr, buf, 0, size);

        Marshal.FreeHGlobal(ptr);
        return buf;
    }
}
