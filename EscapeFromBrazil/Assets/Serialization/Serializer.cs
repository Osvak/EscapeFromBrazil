using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Serializer : MonoBehaviour
{
    [SerializeField] static MemoryStream stream = new MemoryStream();
    bool ser = true;
    byte[] bytes;
    public GameObject go;

    public class CustomClass
    {
        public string name;
        public double posX;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(ser == true)
        {
            Serialize();
            Deserialize();
            ser = false;
        }
    }

    void Serialize()
    {
        var customClass = new CustomClass();
        customClass.name = go.name;
        customClass.posX = go.GetComponent<Transform>().position.x;

        //stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);

        writer.Write(customClass.name);
        writer.Write(customClass.posX);

        bytes = stream.ToArray();
        Debug.Log("Serialized!");
    }
    void Deserialize()
    {
        //stream = new MemoryStream();
        stream.Write(bytes, 0, bytes.Length);
        BinaryReader reader = new BinaryReader(stream);

        stream.Seek(0, SeekOrigin.Begin);
        string s = reader.ReadString();
        Debug.Log("Deserialized: " + s.ToString());

        double f = reader.ReadDouble();
        Debug.Log("Deserialized: " + f.ToString());
    }
}