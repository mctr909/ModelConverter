using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ModelConverter {
    class MmdPmx : BaseModel {
        #region struct
        struct MMD_VERT {
            public vec3 Vert;
            public vec3 Norm;
            public float[] UV;
            public float[,] AdditionalUV;
            public WeightType WeightType;
            public int Bone1;
            public int Bone2;
            public int Bone3;
            public int Bone4;
            public float Weight1;
            public float Weight2;
            public float Weight3;
            public float Weight4;
            public float SdefC;
            public float SdefR0;
            public float SdefR1;
            public float Edge;
        }

        struct MMD_MAT {
            public string Name;
            public string NameEng;

            public float[] Diffuse;
            public float[] Specular;
            public float SpecularPower;
            public float[] Ambient;

            public DisplayFlag Flag;
            public float[] EdgeColor;
            public float EdgeSize;

            public int TextureIndex;
            public int SphereTextureIndex;
            public SphereMode SphereMode;
            public bool ToonFlag;
            public int ToonTextureIndex;

            public string Text;

            public int Vertices;

            public MMD_MAT(int colorIdx = 0) {
                Name = "";
                NameEng = "";

                float[] col = new float[3];
                switch (colorIdx % 5) {
                case 0:
                    col = new float[] { 0, 0.75f, 0 };
                    break;
                case 1:
                    col = new float[] { 0, 0, 0.75f };
                    break;
                case 2:
                    col = new float[] { 0.75f, 0, 0 };
                    break;
                case 3:
                    col = new float[] { 0, 0.66f, 0.66f };
                    break;
                case 4:
                    col = new float[] { 0.66f, 0.66f, 0 };
                    break;
                }

                Diffuse = new float[] { col[0], col[1], col[2], 1 };
                Specular = new float[] { 1.0f, 1.0f, 1.0f };
                SpecularPower = 8;
                Ambient = new float[] { col[0], col[1], col[2] };

                Flag = new DisplayFlag(DisplayFlag.EBoth);
                EdgeColor = new float[] { 0, 0, 0, 1 };
                EdgeSize = 1.0f;

                TextureIndex = 0;
                SphereTextureIndex = 0;
                SphereMode = SphereMode.None;
                ToonFlag = false;
                ToonTextureIndex = 0;

                Text = "";
                Vertices = 0;
            }
        }

        class DisplayFlag {
            public const byte EBoth = 0x1;
            public const byte EGroundShadow = 0x2;
            public const byte ESelfShadowMap = 0x4;
            public const byte ESelfShadow = 0x8;
            public const byte EEdge = 0x10;

            const byte MASK = 0xFF;

            public byte Value { get; private set; }

            public DisplayFlag(byte flag) { Value = flag; }

            public bool Both {
                get { return 0 < (Value & EBoth); }
                set {
                    if (value) {
                        Value |= EBoth;
                    } else {
                        Value &= MASK ^ EBoth;
                    }
                }
            }

            public bool GroundShadow {
                get { return 0 < (Value & EGroundShadow); }
                set {
                    if (value) {
                        Value |= EGroundShadow;
                    } else {
                        Value &= MASK ^ EGroundShadow;
                    }
                }
            }

            public bool SelfShadowMap {
                get { return 0 < (Value & ESelfShadowMap); }
                set {
                    if (value) {
                        Value |= ESelfShadowMap;
                    } else {
                        Value &= MASK ^ ESelfShadowMap;
                    }
                }
            }

            public bool SelfShadow {
                get { return 0 < (Value & ESelfShadow); }
                set {
                    if (value) {
                        Value |= ESelfShadow;
                    } else {
                        Value &= MASK ^ ESelfShadow;
                    }
                }
            }

            public bool Edge {
                get { return 0 < (Value & EEdge); }
                set {
                    if (value) {
                        Value |= EEdge;
                    } else {
                        Value &= MASK ^ EEdge;
                    }
                }
            }
        }

        struct Bone {
            public string Name;
            public string NameEng;
            public vec3 Pos;
            public int ParentIndex;
            public int Layer;
            public BoneFlag Flag;
            public vec3 Offset;
            public int JoinTargetIndex;
            public int AssignmentIndex;
            public float AssignmentRatio;
            public vec3 Direction;
            public vec3 DirectionX;
            public vec3 DirectionZ;
            public int KeyValue;
            public int IkTargetIndex;
            public int IkLoop;
            public float IkLoopAngleLimit;
            public List<IkLink> IkLinks;
        }

        public class BoneFlag {
            const ushort EJointTarget = 0x1;
            const ushort ERotatable = 0x2;
            const ushort EMovable = 0x4;
            const ushort EDisplay = 0x8;

            const ushort EOperable = 0x10;
            const ushort EIK = 0x20;
            const ushort ELocalAssign = 0x80;

            const ushort ERotateAssign = 0x100;
            const ushort EMoveAssign = 0x200;
            const ushort EFixAxiz = 0x400;
            const ushort ELocalAxiz = 0x800;

            const ushort EAfterCalc = 0x1000;
            const ushort EExternalParent = 0x2000;

            const ushort MASK = 0xFFFF;

            public ushort Value { get; private set; }

            public BoneFlag(ushort flag) { Value = flag; }

            public bool JointTarget {
                get { return 0 < (Value & EJointTarget); }
                set {
                    if (value) {
                        Value |= EJointTarget;
                    } else {
                        Value &= MASK ^ EJointTarget;
                    }
                }
            }
            public bool Rotatable {
                get { return 0 < (Value & ERotatable); }
                set {
                    if (value) {
                        Value |= ERotatable;
                    } else {
                        Value &= MASK ^ ERotatable;
                    }
                }
            }
            public bool Movable {
                get { return 0 < (Value & EMovable); }
                set {
                    if (value) {
                        Value |= EMovable;
                    } else {
                        Value &= MASK ^ EMovable;
                    }
                }
            }
            public bool Display {
                get { return 0 < (Value & EDisplay); }
                set {
                    if (value) {
                        Value |= EDisplay;
                    } else {
                        Value &= MASK ^ EDisplay;
                    }
                }
            }

            public bool Operable {
                get { return 0 < (Value & EOperable); }
                set {
                    if (value) {
                        Value |= EOperable;
                    } else {
                        Value &= MASK ^ EOperable;
                    }
                }
            }
            public bool IK {
                get { return 0 < (Value & EIK); }
                set {
                    if (value) {
                        Value |= EIK;
                    } else {
                        Value &= MASK ^ EIK;
                    }
                }
            }
            public bool LocalAssign {
                get { return 0 < (Value & ELocalAssign); }
                set {
                    if (value) {
                        Value |= ELocalAssign;
                    } else {
                        Value &= MASK ^ ELocalAssign;
                    }
                }
            }

            public bool RotateAssign {
                get { return 0 < (Value & ERotateAssign); }
                set {
                    if (value) {
                        Value |= ERotateAssign;
                    } else {
                        Value &= MASK ^ ERotateAssign;
                    }
                }
            }
            public bool MoveAssign {
                get { return 0 < (Value & EMoveAssign); }
                set {
                    if (value) {
                        Value |= EMoveAssign;
                    } else {
                        Value &= MASK ^ EMoveAssign;
                    }
                }
            }
            public bool FixAxiz {
                get { return 0 < (Value & EFixAxiz); }
                set {
                    if (value) {
                        Value |= EFixAxiz;
                    } else {
                        Value &= MASK ^ EFixAxiz;
                    }
                }
            }
            public bool LocalAxiz {
                get { return 0 < (Value & ELocalAxiz); }
                set {
                    if (value) {
                        Value |= ELocalAxiz;
                    } else {
                        Value &= MASK ^ ELocalAxiz;
                    }
                }
            }

            public bool AfterCalc {
                get { return 0 < (Value & EAfterCalc); }
                set {
                    if (value) {
                        Value |= EAfterCalc;
                    } else {
                        Value &= MASK ^ EAfterCalc;
                    }
                }
            }
            public bool ExternalParent {
                get { return 0 < (Value & EExternalParent); }
                set {
                    if (value) {
                        Value |= EExternalParent;
                    } else {
                        Value &= MASK ^ EExternalParent;
                    }
                }
            }
        }

        struct IkLink {
            public int LinkBoneIndex;
            public byte AngleLimit;
            public vec3 AngleMin;
            public vec3 AngleMax;
        }

        struct Morphe {
            public string Name;
            public string NameEng;
            public OperatePanel OperatePanel;
            public MorpheType Type;

            public List<GroupMorphe> Groups;
            public List<VertexMorphe> Vertices;
            public List<BoneMorphe> Bones;
            public List<UVMorphe> UVs;
            public List<UVMorphe> AdditionalUV1s;
            public List<UVMorphe> AdditionalUV2s;
            public List<UVMorphe> AdditionalUV3s;
            public List<UVMorphe> AdditionalUV4s;
            public List<MaterialMorphe> Materials;
        }

        class GroupMorphe {
            public int Index;
            public float Ratio;
            public GroupMorphe(BinaryReader br, int index) {
                Index = index;
                Ratio = br.ReadSingle();
            }
            public void Save(BinaryWriter bw) {
                bw.Write(Ratio);
            }
        }

        class VertexMorphe {
            public int Index;
            public vec3 Offset;
            public VertexMorphe(BinaryReader br, int index) {
                Index = index;
                Offset = new vec3(
                    br.ReadSingle(),
                    br.ReadSingle(),
                    br.ReadSingle()
                );
            }
            public void Save(BinaryWriter bw) {
                bw.Write(Offset.x);
                bw.Write(Offset.y);
                bw.Write(Offset.z);
            }
        }

        class BoneMorphe {
            public int Index;
            public vec3 Offset;
            public float[] Rotate;
            public BoneMorphe(BinaryReader br, int index) {
                Index = index;
                Offset = new vec3(
                    br.ReadSingle(),
                    br.ReadSingle(),
                    br.ReadSingle()
                );
                Rotate = new float[] {
                    br.ReadSingle(),
                    br.ReadSingle(),
                    br.ReadSingle(),
                    br.ReadSingle()
                };
            }
            public void Save(BinaryWriter bw) {
                bw.Write(Offset.x);
                bw.Write(Offset.y);
                bw.Write(Offset.z);

                bw.Write(Rotate[0]);
                bw.Write(Rotate[1]);
                bw.Write(Rotate[2]);
                bw.Write(Rotate[3]);
            }
        }

        class UVMorphe {
            public int Index;
            public float[] Offset;
            public UVMorphe(BinaryReader br, int index) {
                Index = index;
                Offset = new float[] {
                    br.ReadSingle(),
                    br.ReadSingle(),
                    br.ReadSingle(),
                    br.ReadSingle()
                };
            }
            public void Save(BinaryWriter bw) {
                bw.Write(Offset[0]);
                bw.Write(Offset[1]);
                bw.Write(Offset[2]);
                bw.Write(Offset[3]);
            }
        }

        class MaterialMorphe {
            public int Index;
            public OffsetType OffsetType;
            public float[] Diffuse;
            public float[] Specular;
            public float SpecularPower;
            public float[] Ambient;
            public float[] Edge;
            public float EdgeSize;
            public float[] TexturePower;
            public float[] SpherePower;
            public float[] ToonPower;

            public MaterialMorphe(BinaryReader br, int index) {
                Index = index;
                OffsetType = (OffsetType)br.ReadByte();

                Diffuse = new float[] {
                    br.ReadSingle(),
                    br.ReadSingle(),
                    br.ReadSingle(),
                    br.ReadSingle()
                };
                Specular = new float[] {
                    br.ReadSingle(),
                    br.ReadSingle(),
                    br.ReadSingle()
                };
                SpecularPower = br.ReadSingle();
                Ambient = new float[] {
                    br.ReadSingle(),
                    br.ReadSingle(),
                    br.ReadSingle()
                };

                Edge = new float[] {
                    br.ReadSingle(),
                    br.ReadSingle(),
                    br.ReadSingle(),
                    br.ReadSingle()
                };
                EdgeSize = br.ReadSingle();

                TexturePower = new float[] {
                    br.ReadSingle(),
                    br.ReadSingle(),
                    br.ReadSingle(),
                    br.ReadSingle()
                };
                SpherePower = new float[] {
                    br.ReadSingle(),
                    br.ReadSingle(),
                    br.ReadSingle(),
                    br.ReadSingle()
                };
                ToonPower = new float[] {
                    br.ReadSingle(),
                    br.ReadSingle(),
                    br.ReadSingle(),
                    br.ReadSingle()
                };
            }

            public void Save(BinaryWriter bw) {
                bw.Write((byte)OffsetType);

                bw.Write(Diffuse[0]);
                bw.Write(Diffuse[1]);
                bw.Write(Diffuse[2]);
                bw.Write(Diffuse[3]);

                bw.Write(Specular[0]);
                bw.Write(Specular[1]);
                bw.Write(Specular[2]);
                bw.Write(SpecularPower);

                bw.Write(Ambient[0]);
                bw.Write(Ambient[1]);
                bw.Write(Ambient[2]);

                bw.Write(Edge[0]);
                bw.Write(Edge[1]);
                bw.Write(Edge[2]);
                bw.Write(Edge[3]);
                bw.Write(EdgeSize);

                bw.Write(TexturePower[0]);
                bw.Write(TexturePower[1]);
                bw.Write(TexturePower[2]);
                bw.Write(TexturePower[3]);

                bw.Write(SpherePower[0]);
                bw.Write(SpherePower[1]);
                bw.Write(SpherePower[2]);
                bw.Write(SpherePower[3]);

                bw.Write(ToonPower[0]);
                bw.Write(ToonPower[1]);
                bw.Write(ToonPower[2]);
                bw.Write(ToonPower[3]);
            }
        }

        struct DisplayGroup {
            public string Name;
            public string NameEng;
            public bool IsSpecial;
            public List<GroupItem> List;
        }

        struct GroupItem {
            public GroupType Type;
            public int Index;
        }

        struct Rigid {
            public string Name;
            public string NameEng;

            public int BoneIndex;

            public byte Gourp;
            public CollisionFlag CollisionFlag;

            public FormType FormType;
            public vec3 Size;
            public vec3 Pos;
            public vec3 Rotate;

            public float Mass;
            public float MoveResistance;
            public float RotateResistance;
            public float Repulsion;
            public float Friction;

            public CalcType CalcType;
        }

        class CollisionFlag {
            const ushort G1 = 0x1;
            const ushort G2 = 0x2;
            const ushort G3 = 0x4;
            const ushort G4 = 0x8;

            const ushort G5 = 0x10;
            const ushort G6 = 0x20;
            const ushort G7 = 0x40;
            const ushort G8 = 0x80;

            const ushort G9 = 0x100;
            const ushort G10 = 0x200;
            const ushort G11 = 0x400;
            const ushort G12 = 0x800;

            const ushort G13 = 0x1000;
            const ushort G14 = 0x2000;
            const ushort G15 = 0x4000;
            const ushort G16 = 0x8000;

            public ushort Value { get; private set; }

            public bool Group1 {
                get { return 0 < (Value & G1); }
                set {
                    if (value) {
                        Value |= G1;
                    } else {
                        Value &= 0xFFFF ^ G1;
                    }
                }
            }
            public bool Group2 {
                get { return 0 < (Value & G2); }
                set {
                    if (value) {
                        Value |= G2;
                    } else {
                        Value &= 0xFFFF ^ G2;
                    }
                }
            }
            public bool Group3 {
                get { return 0 < (Value & G3); }
                set {
                    if (value) {
                        Value |= G3;
                    } else {
                        Value &= 0xFFFF ^ G3;
                    }
                }
            }
            public bool Group4 {
                get { return 0 < (Value & G4); }
                set {
                    if (value) {
                        Value |= G4;
                    } else {
                        Value &= 0xFFFF ^ G4;
                    }
                }
            }

            public bool Group5 {
                get { return 0 < (Value & G5); }
                set {
                    if (value) {
                        Value |= G5;
                    } else {
                        Value &= 0xFFFF ^ G5;
                    }
                }
            }
            public bool Group6 {
                get { return 0 < (Value & G6); }
                set {
                    if (value) {
                        Value |= G6;
                    } else {
                        Value &= 0xFFFF ^ G6;
                    }
                }
            }
            public bool Group7 {
                get { return 0 < (Value & G7); }
                set {
                    if (value) {
                        Value |= G7;
                    } else {
                        Value &= 0xFFFF ^ G7;
                    }
                }
            }
            public bool Group8 {
                get { return 0 < (Value & G8); }
                set {
                    if (value) {
                        Value |= G8;
                    } else {
                        Value &= 0xFFFF ^ G8;
                    }
                }
            }

            public bool Group9 {
                get { return 0 < (Value & G9); }
                set {
                    if (value) {
                        Value |= G9;
                    } else {
                        Value &= 0xFFFF ^ G9;
                    }
                }
            }
            public bool Group10 {
                get { return 0 < (Value & G10); }
                set {
                    if (value) {
                        Value |= G10;
                    } else {
                        Value &= 0xFFFF ^ G10;
                    }
                }
            }
            public bool Group11 {
                get { return 0 < (Value & G11); }
                set {
                    if (value) {
                        Value |= G11;
                    } else {
                        Value &= 0xFFFF ^ G11;
                    }
                }
            }
            public bool Group12 {
                get { return 0 < (Value & G12); }
                set {
                    if (value) {
                        Value |= G12;
                    } else {
                        Value &= 0xFFFF ^ G12;
                    }
                }
            }

            public bool Group13 {
                get { return 0 < (Value & G13); }
                set {
                    if (value) {
                        Value |= G13;
                    } else {
                        Value &= 0xFFFF ^ G13;
                    }
                }
            }
            public bool Group14 {
                get { return 0 < (Value & G14); }
                set {
                    if (value) {
                        Value |= G14;
                    } else {
                        Value &= 0xFFFF ^ G14;
                    }
                }
            }
            public bool Group15 {
                get { return 0 < (Value & G15); }
                set {
                    if (value) {
                        Value |= G15;
                    } else {
                        Value &= 0xFFFF ^ G15;
                    }
                }
            }
            public bool Group16 {
                get { return 0 < (Value & G16); }
                set {
                    if (value) {
                        Value |= G16;
                    } else {
                        Value &= 0xFFFF ^ G16;
                    }
                }
            }

            public CollisionFlag(ushort value) {
                Value = value;
            }
        }

        struct Joint {
            public string Name;
            public string NameEng;

            public JointType Type;

            public int IndexA;
            public int IndexB;

            public vec3 Pos;
            public vec3 Rotate;

            public vec3 MoveMin;
            public vec3 MoveMax;
            public vec3 RotateMin;
            public vec3 RotateMax;

            public vec3 SpringMove;
            public vec3 SpringRotate;
        }
        #endregion

        #region enum
        enum WeightType : byte {
            BDEF1 = 0,
            BDEF2 = 1,
            BDEF4 = 2,
            SDEF = 3
        }

        enum SphereMode : byte {
            None = 0,
            Mul = 1,
            Add = 2,
            SubTexture = 3
        }

        enum OperatePanel : byte {
            Reserved = 0,
            Mayu = 1,
            Me = 2,
            Kuchi = 3,
            Other = 4
        }

        enum MorpheType : byte {
            Group = 0,
            Vertex = 1,
            Bone = 2,
            UV = 3,
            AdditionalUV1 = 4,
            AdditionalUV2 = 5,
            AdditionalUV3 = 6,
            AdditionalUV4 = 7,
            Material = 8
        }

        enum OffsetType : byte {
            Mul = 0,
            Add = 1
        }

        enum GroupType : byte {
            Bone = 0,
            Morphe = 1
        }

        enum FormType : byte {
            Sphere = 0,
            Box = 1,
            Capsule = 2
        }

        enum CalcType : byte {
            BoneTracking = 0,
            Calc = 1,
            BoneTracking_Calc = 2
        }

        enum JointType : byte {
            SixDOFSpring = 0
        }
        #endregion

        class Header {
            const uint MAGIC_ID = 0x20584D50;
            const float VERSION = 2.0f;

            byte mEncode = 0;
            byte mAdditionalUV = 0;
            byte mVertexIndexSize = 4;
            byte mTextureIndexSize = 4;
            byte mMaterialIndexSize = 4;
            byte mBoneIndexSize = 4;
            byte mMorpheIndexSize = 4;
            byte mRigidIndexSize = 4;

            Encoding Encode { get { return 0 == mEncode ? Encoding.Unicode : Encoding.UTF8; } }

            public byte AdditionalUV { get { return mAdditionalUV; } }

            public Header() { }

            public Header(BinaryReader br) {
                br.ReadUInt32();
                br.ReadSingle();
                var size = br.ReadByte();
                var data = br.ReadBytes(size);
                mEncode = data[0];
                mAdditionalUV = data[1];
                mVertexIndexSize = data[2];
                mTextureIndexSize = data[3];
                mMaterialIndexSize = data[4];
                mBoneIndexSize = data[5];
                mMorpheIndexSize = data[6];
                mRigidIndexSize = data[7];
            }

            public void Write(BinaryWriter bw) {
                bw.Write(MAGIC_ID);
                bw.Write(VERSION);
                bw.Write((byte)8);

                bw.Write(mEncode);
                bw.Write(mAdditionalUV);
                bw.Write(mVertexIndexSize);
                bw.Write(mTextureIndexSize);

                bw.Write(mMaterialIndexSize);
                bw.Write(mBoneIndexSize);
                bw.Write(mMorpheIndexSize);
                bw.Write(mRigidIndexSize);
            }

            public string GetString(byte[] data) {
                return Encode.GetString(data);
            }

            public byte[] GetBytes(string str) {
                return Encode.GetBytes(str);
            }

            public int VertexIndex(BinaryReader br) {
                switch (mVertexIndexSize) {
                case 1:
                    return br.ReadByte();
                case 2:
                    return br.ReadUInt16();
                case 4:
                    return br.ReadInt32();
                default:
                    return -1;
                }
            }

            public void VertexIndex(BinaryWriter bw, int index) {
                switch (mVertexIndexSize) {
                case 1:
                    bw.Write((byte)index);
                    break;
                case 2:
                    bw.Write((ushort)index);
                    break;
                case 4:
                    bw.Write(index);
                    break;
                default:
                    break;
                }
            }

            public int TextureIndex(BinaryReader br) {
                switch (mTextureIndexSize) {
                case 1:
                    return br.ReadByte();
                case 2:
                    return br.ReadUInt16();
                case 4:
                    return br.ReadInt32();
                default:
                    return -1;
                }
            }

            public void TextureIndex(BinaryWriter bw, int index) {
                switch (mTextureIndexSize) {
                case 1:
                    bw.Write((byte)index);
                    break;
                case 2:
                    bw.Write((ushort)index);
                    break;
                case 4:
                    bw.Write(index);
                    break;
                default:
                    break;
                }
            }

            public int MaterialIndex(BinaryReader br) {
                switch (mMaterialIndexSize) {
                case 1:
                    return br.ReadByte();
                case 2:
                    return br.ReadUInt16();
                case 4:
                    return br.ReadInt32();
                default:
                    return -1;
                }
            }

            public void MaterialIndex(BinaryWriter bw, int index) {
                switch (mMaterialIndexSize) {
                case 1:
                    bw.Write((byte)index);
                    break;
                case 2:
                    bw.Write((ushort)index);
                    break;
                case 4:
                    bw.Write(index);
                    break;
                default:
                    break;
                }
            }

            public int BoneIndex(BinaryReader br) {
                switch (mBoneIndexSize) {
                case 1:
                    return br.ReadByte();
                case 2:
                    return br.ReadUInt16();
                case 4:
                    return br.ReadInt32();
                default:
                    return -1;
                }
            }

            public void BoneIndex(BinaryWriter bw, int index) {
                switch (mBoneIndexSize) {
                case 1:
                    bw.Write((byte)index);
                    break;
                case 2:
                    bw.Write((ushort)index);
                    break;
                case 4:
                    bw.Write(index);
                    break;
                default:
                    break;
                }
            }

            public int MorpheIndex(BinaryReader br) {
                switch (mMorpheIndexSize) {
                case 1:
                    return br.ReadByte();
                case 2:
                    return br.ReadUInt16();
                case 4:
                    return br.ReadInt32();
                default:
                    return -1;
                }
            }

            public void MorpheIndex(BinaryWriter bw, int index) {
                switch (mMorpheIndexSize) {
                case 1:
                    bw.Write((byte)index);
                    break;
                case 2:
                    bw.Write((ushort)index);
                    break;
                case 4:
                    bw.Write(index);
                    break;
                default:
                    break;
                }
            }

            public int RigidIndex(BinaryReader br) {
                switch (mRigidIndexSize) {
                case 1:
                    return br.ReadByte();
                case 2:
                    return br.ReadUInt16();
                case 4:
                    return br.ReadInt32();
                default:
                    return -1;
                }
            }

            public void RigidIndex(BinaryWriter bw, int index) {
                switch (mRigidIndexSize) {
                case 1:
                    bw.Write((byte)index);
                    break;
                case 2:
                    bw.Write((ushort)index);
                    break;
                case 4:
                    bw.Write(index);
                    break;
                default:
                    break;
                }
            }
        }

        Header mHeader = new Header();

        string mFilePath;
        string mModelName;
        string mModelNameEng;
        string mComment;
        string mCommentEng;

        List<MMD_VERT> mVertices = new List<MMD_VERT>();
        List<int[]> mFaces = new List<int[]>();
        List<string> mTextures = new List<string>();
        List<MMD_MAT> mMaterials = new List<MMD_MAT>();
        List<Bone> mBones = new List<Bone>();
        List<Morphe> mMorphes = new List<Morphe>();
        List<DisplayGroup> mDisplayGroups = new List<DisplayGroup>();
        List<Rigid> mRigids = new List<Rigid>();
        List<Joint> mJoints = new List<Joint>();

        public MmdPmx() { }

        public MmdPmx(string path) {
            var fs = new FileStream(path, FileMode.Open);
            var br = new BinaryReader(fs);
            mHeader = new Header(br);
            loadModelInfo(br);
            loadVertex(br);
            loadFace(br);
            loadTexture(br);
            loadMaterial(br);
            loadBone(br);
            loadMorphe(br);
            loadDisplayGroup(br);
            loadRigid(br);
            loadJoints(br);
            fs.Close();
            fs.Dispose();

            foreach (var v in mVertices) {
                mVertList.Add(v.Vert);
                mUvList.Add(v.UV);
            }

            var fIdxOfs = 0;
            foreach (var m in mMaterials) {
                var obj = new Object();
                obj.Name = m.Name;
                var faces = m.Vertices / 3;
                for (int fIdx = fIdxOfs; fIdx < fIdxOfs + faces; fIdx++) {
                    var f = mFaces[fIdx];
                    var surf = new Surface();
                    surf.MaterialName = m.Name;
                    surf.Indices.Add(new Index(f[0], f[0]));
                    surf.Indices.Add(new Index(f[1], f[1]));
                    surf.Indices.Add(new Index(f[2], f[2]));
                    obj.Surfaces.Add(surf);
                }
                fIdxOfs += faces;
                mObjectList.Add(obj);

                var mat = new Material();
                mat.Name = m.Name;
                mat.Diffuse = new vec3(m.Diffuse[0], m.Diffuse[1], m.Diffuse[2]);
                mat.Ambient = new vec3(m.Ambient[0], m.Ambient[1], m.Ambient[2]);
                mat.Specular = new vec3(m.Specular[0], m.Specular[1], m.Specular[2]);
                mat.SpecularPower = m.SpecularPower;
                mat.Alpha = m.Diffuse[3];
                mMaterialList.Add(mat.Name, mat);
            }
            Reverse();
        }

        public override void Load(BaseModel srcModel) {
            base.Load(srcModel);
            Reverse();
            ToTriangle();

            for (int i = 0; i < mVertList.Count; i++) {
                var v = mVertList[i];
                var vert = new MMD_VERT();
                vert.Vert = v;
                vert.Norm = new vec3(0, 0, 0);
                vert.UV = mVertList.Count == mUvList.Count ? mUvList[i] : new float[] { 0, 0 };
                mVertices.Add(vert);
            }

            foreach (var o in mObjectList) {
                foreach (var s in o.Surfaces) {
                    for (int i = 0; i < s.Indices.Count; i += 3) {
                        var i0 = s.Indices[i];
                        var i1 = s.Indices[i + 1];
                        var i2 = s.Indices[i + 2];
                        mFaces.Add(new int[] { i0.Vert, i1.Vert, i2.Vert });
                    }
                }
            }

            foreach (var o in mObjectList) {
                var matDic = new Dictionary<string, List<Surface>>();
                foreach (var s in o.Surfaces) {
                    if (0 == s.Indices.Count) {
                        continue;
                    }
                    if (!matDic.ContainsKey(s.MaterialName)) {
                        matDic.Add(s.MaterialName, new List<Surface>());
                    }
                    var surfList = matDic[s.MaterialName];
                    surfList.Add(s);
                }
                foreach (var m in matDic) {
                    var matList = m.Value;
                    var tmpMat = new MMD_MAT(mMaterials.Count);
                    tmpMat.Name = o.Name + "_" + m.Key;
                    tmpMat.NameEng = tmpMat.Name;
                    if (mMaterialList.ContainsKey(matList[0].MaterialName)) {
                        var mat = mMaterialList[matList[0].MaterialName];
                        tmpMat.Diffuse = new float[] { mat.Diffuse.x, mat.Diffuse.y, mat.Diffuse.z, mat.Alpha };
                        tmpMat.Ambient = new float[] { mat.Ambient.x, mat.Ambient.y, mat.Ambient.z };
                        tmpMat.Specular = new float[] { mat.Specular.x, mat.Specular.y, mat.Specular.z };
                        tmpMat.SpecularPower = mat.SpecularPower;
                    }
                    foreach (var s in matList) {
                        tmpMat.Vertices += s.Indices.Count;
                    }
                    mMaterials.Add(tmpMat);
                }
            }
        }

        public override void Save(string path) {
            mFilePath = path;
            var fs = new FileStream(path, FileMode.Create);
            var bw = new BinaryWriter(fs);
            mHeader.Write(bw);
            writeModelInfo(bw);
            writeVertex(bw);
            writeFace(bw);
            writeTexture(bw);
            writeMaterial(bw);
            writeBone(bw);
            writeMorphe(bw);
            writeDisplayGroup(bw);
            writeRigid(bw);
            writeJoints(bw);
            fs.Close();
            fs.Dispose();
        }

        void loadModelInfo(BinaryReader br) {
            var size = br.ReadInt32();
            mModelName = mHeader.GetString(br.ReadBytes(size));
            size = br.ReadInt32();
            mModelNameEng = mHeader.GetString(br.ReadBytes(size));
            size = br.ReadInt32();
            mComment = mHeader.GetString(br.ReadBytes(size));
            size = br.ReadInt32();
            mCommentEng = mHeader.GetString(br.ReadBytes(size));
        }

        void writeModelInfo(BinaryWriter bw) {
            if (string.IsNullOrEmpty(mModelName)) {
                mModelName = Path.GetFileNameWithoutExtension(mFilePath);
            }
            if (string.IsNullOrEmpty(mModelNameEng)) {
                mModelNameEng = mModelName;
            }
            if (null == mComment) {
                mComment = "";
            }
            if (null == mCommentEng) {
                mCommentEng = "";
            }
            var arr = mHeader.GetBytes(mModelName);
            bw.Write(arr.Length);
            bw.Write(arr);
            arr = mHeader.GetBytes(mModelNameEng);
            bw.Write(arr.Length);
            bw.Write(arr);
            arr = mHeader.GetBytes(mComment);
            bw.Write(arr.Length);
            bw.Write(arr);
            arr = mHeader.GetBytes(mCommentEng);
            bw.Write(arr.Length);
            bw.Write(arr);
        }

        void loadVertex(BinaryReader br) {
            var count = br.ReadInt32();

            for (int i = 0; i < count; i++) {
                var vertex = new MMD_VERT();
                vertex.Vert = new vec3(
                    br.ReadSingle(),
                    br.ReadSingle(),
                    br.ReadSingle()
                );
                vertex.Norm = new vec3(
                    br.ReadSingle(),
                    br.ReadSingle(),
                    br.ReadSingle()
                );
                vertex.UV = new float[] {
                    br.ReadSingle(),
                    br.ReadSingle()
                };
                vertex.AdditionalUV = new float[mHeader.AdditionalUV, 4];
                for (int j = 0; j < mHeader.AdditionalUV; j++) {
                    vertex.AdditionalUV[j, 0] = br.ReadSingle();
                    vertex.AdditionalUV[j, 1] = br.ReadSingle();
                    vertex.AdditionalUV[j, 2] = br.ReadSingle();
                    vertex.AdditionalUV[j, 3] = br.ReadSingle();
                }

                vertex.WeightType = (WeightType)br.ReadByte();

                switch (vertex.WeightType) {
                case WeightType.BDEF1:
                    vertex.Bone1 = mHeader.BoneIndex(br);
                    break;
                case WeightType.BDEF2:
                    vertex.Bone1 = mHeader.BoneIndex(br);
                    vertex.Bone2 = mHeader.BoneIndex(br);
                    vertex.Weight1 = br.ReadSingle();
                    vertex.Weight2 = 1.0f - vertex.Weight1;
                    break;
                case WeightType.BDEF4:
                    vertex.Bone1 = mHeader.BoneIndex(br);
                    vertex.Bone2 = mHeader.BoneIndex(br);
                    vertex.Bone3 = mHeader.BoneIndex(br);
                    vertex.Bone4 = mHeader.BoneIndex(br);
                    vertex.Weight1 = br.ReadSingle();
                    vertex.Weight2 = br.ReadSingle();
                    vertex.Weight3 = br.ReadSingle();
                    vertex.Weight4 = br.ReadSingle();
                    break;
                case WeightType.SDEF:
                    vertex.Bone1 = mHeader.BoneIndex(br);
                    vertex.Bone2 = mHeader.BoneIndex(br);
                    vertex.Weight1 = br.ReadSingle();
                    vertex.Weight2 = 1.0f - vertex.Weight1;
                    vertex.SdefC = br.ReadSingle();
                    vertex.SdefR0 = br.ReadSingle();
                    vertex.SdefR1 = br.ReadSingle();
                    break;
                default:
                    break;
                }

                vertex.Edge = br.ReadSingle();
                mVertices.Add(vertex);
            }
        }

        void writeVertex(BinaryWriter bw) {
            bw.Write(mVertices.Count);

            foreach (var vertex in mVertices) {
                bw.Write(vertex.Vert.x);
                bw.Write(vertex.Vert.y);
                bw.Write(vertex.Vert.z);

                bw.Write(vertex.Norm.x);
                bw.Write(vertex.Norm.y);
                bw.Write(vertex.Norm.z);

                bw.Write(vertex.UV[0]);
                bw.Write(vertex.UV[1]);

                for (int j = 0; j < mHeader.AdditionalUV; j++) {
                    bw.Write(vertex.AdditionalUV[j, 0]);
                    bw.Write(vertex.AdditionalUV[j, 1]);
                    bw.Write(vertex.AdditionalUV[j, 2]);
                    bw.Write(vertex.AdditionalUV[j, 3]);
                }

                bw.Write((byte)vertex.WeightType);

                switch (vertex.WeightType) {
                case WeightType.BDEF1:
                    mHeader.BoneIndex(bw, vertex.Bone1);
                    break;
                case WeightType.BDEF2:
                    mHeader.BoneIndex(bw, vertex.Bone1);
                    mHeader.BoneIndex(bw, vertex.Bone2);
                    bw.Write(vertex.Weight1);
                    break;
                case WeightType.BDEF4:
                    mHeader.BoneIndex(bw, vertex.Bone1);
                    mHeader.BoneIndex(bw, vertex.Bone2);
                    mHeader.BoneIndex(bw, vertex.Bone3);
                    mHeader.BoneIndex(bw, vertex.Bone4);
                    bw.Write(vertex.Weight1);
                    bw.Write(vertex.Weight2);
                    bw.Write(vertex.Weight3);
                    bw.Write(vertex.Weight4);
                    break;
                case WeightType.SDEF:
                    mHeader.BoneIndex(bw, vertex.Bone1);
                    mHeader.BoneIndex(bw, vertex.Bone2);
                    bw.Write(vertex.Weight1);
                    bw.Write(vertex.SdefC);
                    bw.Write(vertex.SdefR0);
                    bw.Write(vertex.SdefR1);
                    break;
                default:
                    break;
                }

                bw.Write(vertex.Edge);
            }
        }

        void loadFace(BinaryReader br) {
            var count = br.ReadInt32();
            int[] indeces = null;
            for (int i = 0; i < count; i++) {
                switch (i % 3) {
                case 0:
                    indeces = new int[3];
                    indeces[0] = mHeader.VertexIndex(br);
                    break;
                case 1:
                    indeces[1] = mHeader.VertexIndex(br);
                    break;
                case 2:
                    indeces[2] = mHeader.VertexIndex(br);
                    mFaces.Add(indeces);
                    break;
                }
            }
        }

        void writeFace(BinaryWriter bw) {
            bw.Write(mFaces.Count * 3);
            foreach (var face in mFaces) {
                mHeader.VertexIndex(bw, face[0]);
                mHeader.VertexIndex(bw, face[1]);
                mHeader.VertexIndex(bw, face[2]);
            }
        }

        void loadTexture(BinaryReader br) {
            var count = br.ReadInt32();
            for (int i = 0; i < count; i++) {
                var size = br.ReadInt32();
                mTextures.Add(mHeader.GetString(br.ReadBytes(size)));
            }
        }

        void writeTexture(BinaryWriter bw) {
            bw.Write(mTextures.Count);
            foreach (var texture in mTextures) {
                var arr = mHeader.GetBytes(texture);
                bw.Write(arr.Length);
                bw.Write(arr);
            }
        }

        void loadMaterial(BinaryReader br) {
            var count = br.ReadInt32();
            for (int i = 0; i < count; i++) {
                var mat = new MMD_MAT();

                var size = br.ReadInt32();
                mat.Name = mHeader.GetString(br.ReadBytes(size));
                size = br.ReadInt32();
                mat.NameEng = mHeader.GetString(br.ReadBytes(size));

                mat.Diffuse = new float[] {
                    br.ReadSingle(),
                    br.ReadSingle(),
                    br.ReadSingle(),
                    br.ReadSingle()
                };
                mat.Specular = new float[] {
                    br.ReadSingle(),
                    br.ReadSingle(),
                    br.ReadSingle()
                };
                mat.SpecularPower = br.ReadSingle();
                mat.Ambient = new float[] {
                    br.ReadSingle(),
                    br.ReadSingle(),
                    br.ReadSingle()
                };

                mat.Flag = new DisplayFlag(br.ReadByte());

                mat.EdgeColor = new float[] {
                    br.ReadSingle(),
                    br.ReadSingle(),
                    br.ReadSingle(),
                    br.ReadSingle()
                };
                mat.EdgeSize = br.ReadSingle();

                mat.TextureIndex = mHeader.TextureIndex(br);
                mat.SphereTextureIndex = mHeader.TextureIndex(br);
                mat.SphereMode = (SphereMode)br.ReadByte();
                mat.ToonFlag = br.ReadBoolean();
                if (mat.ToonFlag) {
                    mat.ToonTextureIndex = br.ReadByte();
                } else {
                    mat.ToonTextureIndex = mHeader.TextureIndex(br);
                }

                size = br.ReadInt32();
                mat.Text = mHeader.GetString(br.ReadBytes(size));

                mat.Vertices = br.ReadInt32();
                mMaterials.Add(mat);
            }
        }

        void writeMaterial(BinaryWriter bw) {
            bw.Write(mMaterials.Count);
            foreach (var mat in mMaterials) {
                var arr = mHeader.GetBytes(mat.Name);
                bw.Write(arr.Length);
                bw.Write(arr);
                arr = mHeader.GetBytes(mat.NameEng);
                bw.Write(arr.Length);
                bw.Write(arr);

                bw.Write(mat.Diffuse[0]);
                bw.Write(mat.Diffuse[1]);
                bw.Write(mat.Diffuse[2]);
                bw.Write(mat.Diffuse[3]);

                bw.Write(mat.Specular[0]);
                bw.Write(mat.Specular[1]);
                bw.Write(mat.Specular[2]);
                bw.Write(mat.SpecularPower);

                bw.Write(mat.Ambient[0]);
                bw.Write(mat.Ambient[1]);
                bw.Write(mat.Ambient[2]);

                bw.Write(mat.Flag.Value);

                bw.Write(mat.EdgeColor[0]);
                bw.Write(mat.EdgeColor[1]);
                bw.Write(mat.EdgeColor[2]);
                bw.Write(mat.EdgeColor[3]);
                bw.Write(mat.EdgeSize);

                mHeader.TextureIndex(bw, mat.TextureIndex);
                mHeader.TextureIndex(bw, mat.SphereTextureIndex);
                bw.Write((byte)mat.SphereMode);
                bw.Write(mat.ToonFlag);
                if (mat.ToonFlag) {
                    bw.Write((byte)mat.ToonTextureIndex);
                } else {
                    mHeader.TextureIndex(bw, mat.ToonTextureIndex);
                }

                arr = mHeader.GetBytes(mat.Text);
                bw.Write(arr.Length);
                bw.Write(arr);

                bw.Write(mat.Vertices);
            }
        }

        void loadBone(BinaryReader br) {
            var count = br.ReadInt32();
            for (int i = 0; i < count; i++) {
                var bone = new Bone();

                var size = br.ReadInt32();
                bone.Name = mHeader.GetString(br.ReadBytes(size));
                size = br.ReadInt32();
                bone.NameEng = mHeader.GetString(br.ReadBytes(size));

                bone.Pos = new vec3(
                    br.ReadSingle(),
                    br.ReadSingle(),
                    br.ReadSingle()
                );
                bone.ParentIndex = mHeader.BoneIndex(br);
                bone.Layer = br.ReadInt32();

                bone.Flag = new BoneFlag(br.ReadUInt16());

                if (bone.Flag.JointTarget) {
                    bone.JoinTargetIndex = mHeader.BoneIndex(br);
                } else {
                    bone.Offset = new vec3(
                        br.ReadSingle(),
                        br.ReadSingle(),
                        br.ReadSingle()
                    );
                }

                if (bone.Flag.RotateAssign || bone.Flag.MoveAssign) {
                    bone.AssignmentIndex = mHeader.BoneIndex(br);
                    bone.AssignmentRatio = br.ReadSingle();
                }

                if (bone.Flag.FixAxiz) {
                    bone.Direction = new vec3(
                        br.ReadSingle(),
                        br.ReadSingle(),
                        br.ReadSingle()
                    );
                }

                if (bone.Flag.LocalAxiz) {
                    bone.DirectionX = new vec3(
                        br.ReadSingle(),
                        br.ReadSingle(),
                        br.ReadSingle()
                    );
                    bone.DirectionZ = new vec3(
                        br.ReadSingle(),
                        br.ReadSingle(),
                        br.ReadSingle()
                    );
                }

                if (bone.Flag.ExternalParent) {
                    bone.KeyValue = br.ReadInt32();
                }

                if (bone.Flag.IK) {
                    bone.IkTargetIndex = mHeader.BoneIndex(br);
                    bone.IkLoop = br.ReadInt32();
                    bone.IkLoopAngleLimit = br.ReadSingle();
                    bone.IkLinks = new List<IkLink>();
                    var linkCount = br.ReadInt32();
                    for (int j = 0; j < linkCount; j++) {
                        var link = new IkLink();
                        link.LinkBoneIndex = mHeader.BoneIndex(br);
                        link.AngleLimit = br.ReadByte();
                        if (0 < link.AngleLimit) {
                            link.AngleMin = new vec3(
                                br.ReadSingle(),
                                br.ReadSingle(),
                                br.ReadSingle()
                            );
                            link.AngleMax = new vec3(
                                br.ReadSingle(),
                                br.ReadSingle(),
                                br.ReadSingle()
                            );
                        }
                        bone.IkLinks.Add(link);
                    }
                }

                mBones.Add(bone);
            }
        }

        void writeBone(BinaryWriter bw) {
            bw.Write(mBones.Count);
            foreach (var bone in mBones) {
                var arr = mHeader.GetBytes(bone.Name);
                bw.Write(arr.Length);
                bw.Write(arr);
                arr = mHeader.GetBytes(bone.NameEng);
                bw.Write(arr.Length);
                bw.Write(arr);

                bw.Write(bone.Pos.x);
                bw.Write(bone.Pos.y);
                bw.Write(bone.Pos.z);

                mHeader.BoneIndex(bw, bone.ParentIndex);
                bw.Write(bone.Layer);
                bw.Write(bone.Flag.Value);

                if (bone.Flag.JointTarget) {
                    mHeader.BoneIndex(bw, bone.JoinTargetIndex);
                } else {
                    bw.Write(bone.Offset.x);
                    bw.Write(bone.Offset.y);
                    bw.Write(bone.Offset.z);
                }

                if (bone.Flag.RotateAssign || bone.Flag.MoveAssign) {
                    mHeader.BoneIndex(bw, bone.AssignmentIndex);
                    bw.Write(bone.AssignmentRatio);
                }

                if (bone.Flag.FixAxiz) {
                    bw.Write(bone.Direction.x);
                    bw.Write(bone.Direction.y);
                    bw.Write(bone.Direction.z);
                }

                if (bone.Flag.LocalAxiz) {
                    bw.Write(bone.DirectionX.x);
                    bw.Write(bone.DirectionX.y);
                    bw.Write(bone.DirectionX.z);

                    bw.Write(bone.DirectionZ.x);
                    bw.Write(bone.DirectionZ.y);
                    bw.Write(bone.DirectionZ.z);
                }

                if (bone.Flag.ExternalParent) {
                    bw.Write(bone.KeyValue);
                }

                if (bone.Flag.IK) {
                    mHeader.BoneIndex(bw, bone.IkTargetIndex);
                    bw.Write(bone.IkLoop);
                    bw.Write(bone.IkLoopAngleLimit);
                    bw.Write(bone.IkLinks.Count);
                    foreach (var link in bone.IkLinks) {
                        mHeader.BoneIndex(bw, link.LinkBoneIndex);
                        bw.Write(link.AngleLimit);
                        if (0 < link.AngleLimit) {
                            bw.Write(link.AngleMin.x);
                            bw.Write(link.AngleMin.y);
                            bw.Write(link.AngleMin.z);
                            bw.Write(link.AngleMax.x);
                            bw.Write(link.AngleMax.y);
                            bw.Write(link.AngleMax.z);
                        }
                    }
                }
            }
        }

        void loadMorphe(BinaryReader br) {
            var count = br.ReadInt32();
            for (int i = 0; i < count; i++) {
                var morphe = new Morphe();

                var size = br.ReadInt32();
                morphe.Name = mHeader.GetString(br.ReadBytes(size));
                size = br.ReadInt32();
                morphe.NameEng = mHeader.GetString(br.ReadBytes(size));

                morphe.OperatePanel = (OperatePanel)br.ReadByte();
                morphe.Type = (MorpheType)br.ReadByte();
                var subCount = br.ReadInt32();

                switch (morphe.Type) {
                case MorpheType.Group:
                    morphe.Groups = new List<GroupMorphe>();
                    for (int j = 0; j < subCount; j++) {
                        morphe.Groups.Add(new GroupMorphe(br, mHeader.MorpheIndex(br)));
                    }
                    break;
                case MorpheType.Vertex:
                    morphe.Vertices = new List<VertexMorphe>();
                    for (int j = 0; j < subCount; j++) {
                        morphe.Vertices.Add(new VertexMorphe(br, mHeader.VertexIndex(br)));
                    }
                    break;
                case MorpheType.Bone:
                    morphe.Bones = new List<BoneMorphe>();
                    for (int j = 0; j < subCount; j++) {
                        morphe.Bones.Add(new BoneMorphe(br, mHeader.BoneIndex(br)));
                    }
                    break;
                case MorpheType.UV:
                    morphe.UVs = new List<UVMorphe>();
                    for (int j = 0; j < subCount; j++) {
                        morphe.UVs.Add(new UVMorphe(br, mHeader.VertexIndex(br)));
                    }
                    break;
                case MorpheType.AdditionalUV1:
                    morphe.AdditionalUV1s = new List<UVMorphe>();
                    for (int j = 0; j < subCount; j++) {
                        morphe.AdditionalUV1s.Add(new UVMorphe(br, mHeader.VertexIndex(br)));
                    }
                    break;
                case MorpheType.AdditionalUV2:
                    morphe.AdditionalUV2s = new List<UVMorphe>();
                    for (int j = 0; j < subCount; j++) {
                        morphe.AdditionalUV2s.Add(new UVMorphe(br, mHeader.VertexIndex(br)));
                    }
                    break;
                case MorpheType.AdditionalUV3:
                    morphe.AdditionalUV3s = new List<UVMorphe>();
                    for (int j = 0; j < subCount; j++) {
                        morphe.AdditionalUV3s.Add(new UVMorphe(br, mHeader.VertexIndex(br)));
                    }
                    break;
                case MorpheType.AdditionalUV4:
                    morphe.AdditionalUV4s = new List<UVMorphe>();
                    for (int j = 0; j < subCount; j++) {
                        morphe.AdditionalUV4s.Add(new UVMorphe(br, mHeader.VertexIndex(br)));
                    }
                    break;
                case MorpheType.Material:
                    morphe.Materials = new List<MaterialMorphe>();
                    for (int j = 0; j < subCount; j++) {
                        morphe.Materials.Add(new MaterialMorphe(br, mHeader.MaterialIndex(br)));
                    }
                    break;
                default:
                    break;
                }

                mMorphes.Add(morphe);
            }
        }

        void writeMorphe(BinaryWriter bw) {
            bw.Write(mMorphes.Count);
            foreach (var morphe in mMorphes) {
                var arr = mHeader.GetBytes(morphe.Name);
                bw.Write(arr.Length);
                bw.Write(arr);
                arr = mHeader.GetBytes(morphe.NameEng);
                bw.Write(arr.Length);
                bw.Write(arr);

                bw.Write((byte)morphe.OperatePanel);
                bw.Write((byte)morphe.Type);

                switch (morphe.Type) {
                case MorpheType.Group:
                    bw.Write(morphe.Groups.Count);
                    foreach (var group in morphe.Groups) {
                        mHeader.MorpheIndex(bw, group.Index);
                        group.Save(bw);
                    }
                    break;
                case MorpheType.Vertex:
                    bw.Write(morphe.Vertices.Count);
                    foreach (var vertex in morphe.Vertices) {
                        mHeader.VertexIndex(bw, vertex.Index);
                        vertex.Save(bw);
                    }
                    break;
                case MorpheType.Bone:
                    bw.Write(morphe.Bones.Count);
                    foreach (var bone in morphe.Bones) {
                        mHeader.BoneIndex(bw, bone.Index);
                        bone.Save(bw);
                    }
                    break;
                case MorpheType.UV:
                    bw.Write(morphe.UVs.Count);
                    foreach (var uv in morphe.UVs) {
                        mHeader.VertexIndex(bw, uv.Index);
                        uv.Save(bw);
                    }
                    break;
                case MorpheType.AdditionalUV1:
                    bw.Write(morphe.AdditionalUV1s.Count);
                    foreach (var uv in morphe.AdditionalUV1s) {
                        mHeader.VertexIndex(bw, uv.Index);
                        uv.Save(bw);
                    }
                    break;
                case MorpheType.AdditionalUV2:
                    bw.Write(morphe.AdditionalUV2s.Count);
                    foreach (var uv in morphe.AdditionalUV2s) {
                        mHeader.VertexIndex(bw, uv.Index);
                        uv.Save(bw);
                    }
                    break;
                case MorpheType.AdditionalUV3:
                    bw.Write(morphe.AdditionalUV3s.Count);
                    foreach (var uv in morphe.AdditionalUV3s) {
                        mHeader.VertexIndex(bw, uv.Index);
                        uv.Save(bw);
                    }
                    break;
                case MorpheType.AdditionalUV4:
                    bw.Write(morphe.AdditionalUV4s.Count);
                    foreach (var uv in morphe.AdditionalUV4s) {
                        mHeader.VertexIndex(bw, uv.Index);
                        uv.Save(bw);
                    }
                    break;
                case MorpheType.Material:
                    bw.Write(morphe.Materials.Count);
                    foreach (var mat in morphe.Materials) {
                        mHeader.MaterialIndex(bw, mat.Index);
                        mat.Save(bw);
                    }
                    break;
                default:
                    break;
                }
            }
        }

        void loadDisplayGroup(BinaryReader br) {
            var count = br.ReadInt32();
            for (int i = 0; i < count; i++) {
                var group = new DisplayGroup();

                var size = br.ReadInt32();
                group.Name = mHeader.GetString(br.ReadBytes(size));
                size = br.ReadInt32();
                group.NameEng = mHeader.GetString(br.ReadBytes(size));

                group.IsSpecial = br.ReadBoolean();

                group.List = new List<GroupItem>();
                var subCount = br.ReadInt32();
                for (int j = 0; j < subCount; j++) {
                    var item = new GroupItem();
                    item.Type = (GroupType)br.ReadByte();
                    switch (item.Type) {
                    case GroupType.Bone:
                        item.Index = mHeader.BoneIndex(br);
                        break;
                    case GroupType.Morphe:
                        item.Index = mHeader.MorpheIndex(br);
                        break;
                    default:
                        break;
                    }
                    group.List.Add(item);
                }

                mDisplayGroups.Add(group);
            }
        }

        void writeDisplayGroup(BinaryWriter bw) {
            bw.Write(mDisplayGroups.Count);
            foreach (var group in mDisplayGroups) {
                var arr = mHeader.GetBytes(group.Name);
                bw.Write(arr.Length);
                bw.Write(arr);
                arr = mHeader.GetBytes(group.NameEng);
                bw.Write(arr.Length);
                bw.Write(arr);

                bw.Write(group.IsSpecial);

                bw.Write(group.List.Count);
                foreach (var item in group.List) {
                    bw.Write((byte)item.Type);
                    switch (item.Type) {
                    case GroupType.Bone:
                        mHeader.BoneIndex(bw, item.Index);
                        break;
                    case GroupType.Morphe:
                        mHeader.MorpheIndex(bw, item.Index);
                        break;
                    default:
                        break;
                    }
                }
            }
        }

        void loadRigid(BinaryReader br) {
            var count = br.ReadInt32();
            for (int i = 0; i < count; i++) {
                var rigid = new Rigid();

                var size = br.ReadInt32();
                rigid.Name = mHeader.GetString(br.ReadBytes(size));
                size = br.ReadInt32();
                rigid.NameEng = mHeader.GetString(br.ReadBytes(size));

                rigid.BoneIndex = mHeader.BoneIndex(br);

                rigid.Gourp = br.ReadByte();
                rigid.CollisionFlag = new CollisionFlag(br.ReadUInt16());
                rigid.FormType = (FormType)br.ReadByte();
                rigid.Size = new vec3(
                    br.ReadSingle(),
                    br.ReadSingle(),
                    br.ReadSingle()
                );
                rigid.Pos = new vec3(
                    br.ReadSingle(),
                    br.ReadSingle(),
                    br.ReadSingle()
                );
                rigid.Rotate = new vec3(
                    br.ReadSingle(),
                    br.ReadSingle(),
                    br.ReadSingle()
                );

                rigid.Mass = br.ReadSingle();
                rigid.MoveResistance = br.ReadSingle();
                rigid.RotateResistance = br.ReadSingle();
                rigid.Repulsion = br.ReadSingle();
                rigid.Friction = br.ReadSingle();

                rigid.CalcType = (CalcType)br.ReadByte();

                mRigids.Add(rigid);
            }
        }

        void writeRigid(BinaryWriter bw) {
            bw.Write(mRigids.Count);
            foreach (var rigid in mRigids) {
                var arr = mHeader.GetBytes(rigid.Name);
                bw.Write(arr.Length);
                bw.Write(arr);
                arr = mHeader.GetBytes(rigid.NameEng);
                bw.Write(arr.Length);
                bw.Write(arr);

                mHeader.BoneIndex(bw, rigid.BoneIndex);

                bw.Write(rigid.Gourp);
                bw.Write(rigid.CollisionFlag.Value);
                bw.Write((byte)rigid.FormType);

                bw.Write(rigid.Size.x);
                bw.Write(rigid.Size.y);
                bw.Write(rigid.Size.z);

                bw.Write(rigid.Pos.x);
                bw.Write(rigid.Pos.y);
                bw.Write(rigid.Pos.z);

                bw.Write(rigid.Rotate.x);
                bw.Write(rigid.Rotate.y);
                bw.Write(rigid.Rotate.z);

                bw.Write(rigid.Mass);
                bw.Write(rigid.MoveResistance);
                bw.Write(rigid.RotateResistance);
                bw.Write(rigid.Repulsion);
                bw.Write(rigid.Friction);

                bw.Write((byte)rigid.CalcType);
            }
        }

        void loadJoints(BinaryReader br) {
            var count = br.ReadInt32();
            for (int i = 0; i < count; i++) {
                var joint = new Joint();

                var size = br.ReadInt32();
                joint.Name = mHeader.GetString(br.ReadBytes(size));
                size = br.ReadInt32();
                joint.NameEng = mHeader.GetString(br.ReadBytes(size));

                joint.Type = (JointType)br.ReadByte();
                switch (joint.Type) {
                case JointType.SixDOFSpring:
                    joint.IndexA = mHeader.RigidIndex(br);
                    joint.IndexB = mHeader.RigidIndex(br);
                    joint.Pos = new vec3(
                        br.ReadSingle(),
                        br.ReadSingle(),
                        br.ReadSingle()
                    );
                    joint.Rotate = new vec3(
                        br.ReadSingle(),
                        br.ReadSingle(),
                        br.ReadSingle()
                    );
                    joint.MoveMin = new vec3(
                       br.ReadSingle(),
                       br.ReadSingle(),
                       br.ReadSingle()
                    );
                    joint.MoveMax = new vec3(
                       br.ReadSingle(),
                       br.ReadSingle(),
                       br.ReadSingle()
                    );
                    joint.RotateMin = new vec3(
                       br.ReadSingle(),
                       br.ReadSingle(),
                       br.ReadSingle()
                    );
                    joint.RotateMax = new vec3(
                       br.ReadSingle(),
                       br.ReadSingle(),
                       br.ReadSingle()
                    );
                    joint.SpringMove = new vec3(
                       br.ReadSingle(),
                       br.ReadSingle(),
                       br.ReadSingle()
                    );
                    joint.SpringRotate = new vec3(
                       br.ReadSingle(),
                       br.ReadSingle(),
                       br.ReadSingle()
                    );
                    break;
                default:
                    break;
                }

                mJoints.Add(joint);
            }
        }

        void writeJoints(BinaryWriter bw) {
            bw.Write(mJoints.Count);
            foreach (var joint in mJoints) {
                var arr = mHeader.GetBytes(joint.Name);
                bw.Write(arr.Length);
                bw.Write(arr);
                arr = mHeader.GetBytes(joint.NameEng);
                bw.Write(arr.Length);
                bw.Write(arr);

                bw.Write((byte)joint.Type);
                switch (joint.Type) {
                case JointType.SixDOFSpring:
                    mHeader.RigidIndex(bw, joint.IndexA);
                    mHeader.RigidIndex(bw, joint.IndexB);

                    bw.Write(joint.Pos.x);
                    bw.Write(joint.Pos.y);
                    bw.Write(joint.Pos.z);

                    bw.Write(joint.Rotate.x);
                    bw.Write(joint.Rotate.y);
                    bw.Write(joint.Rotate.z);

                    bw.Write(joint.MoveMin.x);
                    bw.Write(joint.MoveMin.y);
                    bw.Write(joint.MoveMin.z);

                    bw.Write(joint.MoveMax.x);
                    bw.Write(joint.MoveMax.y);
                    bw.Write(joint.MoveMax.z);

                    bw.Write(joint.RotateMin.x);
                    bw.Write(joint.RotateMin.y);
                    bw.Write(joint.RotateMin.z);

                    bw.Write(joint.RotateMax.x);
                    bw.Write(joint.RotateMax.y);
                    bw.Write(joint.RotateMax.z);

                    bw.Write(joint.SpringMove.x);
                    bw.Write(joint.SpringMove.y);
                    bw.Write(joint.SpringMove.z);

                    bw.Write(joint.SpringRotate.x);
                    bw.Write(joint.SpringRotate.y);
                    bw.Write(joint.SpringRotate.z);
                    break;
                default:
                    break;
                }
            }
        }
    }
}
