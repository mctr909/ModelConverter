interface IModel {
    abstract void Save(string filePath);
    abstract void Normalize(float scale = 1);
}
