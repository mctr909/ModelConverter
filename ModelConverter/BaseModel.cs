public enum SwapAxiz {
    XYZ,
    XZY,
    YXZ,
    YZX,
    ZXY,
    ZYX
}

public enum InvertAxiz {
    None = 0,
    X = 1,
    Y = 2,
    Z = 4
}

public enum InvertUV {
    None = 0,
    U = 1,
    V = 2
}

abstract class BaseModel {
    public SwapAxiz SwapAxiz = SwapAxiz.XYZ;
    public bool SwapUV = false;
    public InvertAxiz InvertAxiz = InvertAxiz.None;
    public InvertUV InvertUV = InvertUV.None;

    public int SignX { get { return 0 < (InvertAxiz & InvertAxiz.X) ? -1 : 1; } }
    public int SignY { get { return 0 < (InvertAxiz & InvertAxiz.Y) ? -1 : 1; } }
    public int SignZ { get { return 0 < (InvertAxiz & InvertAxiz.Z) ? -1 : 1; } }

    public bool InvertU { get { return 0 < (InvertUV & InvertUV.U); } }
    public bool InvertV { get { return 0 < (InvertUV & InvertUV.V); } }

    public abstract void Save(string path);

    public abstract void Normalize(float scale = 1.0f);
}

