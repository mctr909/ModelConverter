abstract class BaseModel {
    public enum ESwapAxiz {
        XYZ,
        XZY,
        YXZ,
        YZX,
        ZXY,
        ZYX
    }

    public enum EInvertAxiz {
        None = 0,
        X = 1,
        Y = 2,
        Z = 4
    }

    public enum EInvertUV {
        None = 0,
        U = 1,
        V = 2
    }

    public ESwapAxiz SwapAxiz = ESwapAxiz.XYZ;
    public bool SwapUV = false;
    public EInvertAxiz InvertAxiz = EInvertAxiz.None;
    public EInvertUV InvertUV = EInvertUV.None;

    public int SignX { get { return 0 < (InvertAxiz & EInvertAxiz.X) ? -1 : 1; } }
    public int SignY { get { return 0 < (InvertAxiz & EInvertAxiz.Y) ? -1 : 1; } }
    public int SignZ { get { return 0 < (InvertAxiz & EInvertAxiz.Z) ? -1 : 1; } }

    public bool InvertU { get { return 0 < (InvertUV & EInvertUV.U); } }
    public bool InvertV { get { return 0 < (InvertUV & EInvertUV.V); } }

    public abstract void Save(string path);

    public abstract void Normalize(float scale = 1.0f);
}

