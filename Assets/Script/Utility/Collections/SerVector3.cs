using System;
using UnityEngine;

[Serializable]
public class SerVector3
{
    public float x, y, z;

    public SerVector3()
    {
    }

    public SerVector3(float x, float y, float z)
    {
        Set(x, y, z);
    }

    public SerVector3(Vector3 vector)
    {
        Set(vector);
    }

    public void Set(Vector3 vector)
    {
        this.x=vector.x;
        this.y=vector.y;
        this.z=vector.z;
    }

    public void Set(float x, float y, float z)
    {
        this.x=x;
        this.y=y;
        this.z=z;
    }

    public Vector3 Get()
    {
        return new Vector3(x, y, z);
    }
}
