namespace Papyrus.Business.Exporters {
    public class PathByVersionGenerator : PathGenerator {
        public override string GenerateMkdocsPath() {
            return Language + "/" + Version;
        }

        public override string GenerateDocumentRoute() {
            return Product;
        }
    }
}