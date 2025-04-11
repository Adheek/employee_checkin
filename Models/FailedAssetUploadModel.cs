namespace AEET.Models
{
    public class FailedAssetUploadModel
    {
        public BulkUploadTemplateRecordModel AssetDetails { get; set; }
        public string ErrorMessage { get; set; }
    }
}
