namespace Navi.Core.Constants
{
    /// <summary>
    /// Hằng số đường dẫn API cho tất cả controllers
    /// API Endpoints constants for all controllers
    /// </summary>
    public static class ApiEndpoints
    {
        public const string BaseUrl = "https://localhost:7025/api";

        // ── NaviProducts ─────────────────────────────────────────────
        public const string NaviProducts             = "naviproducts";
        public const string NaviProductById          = "naviproducts/{0}";
        public const string NaviProductWithItems     = "naviproducts/{0}/items";
        public const string NaviProductSearch        = "naviproducts/search";
        public const string NaviProductCreateWithItems      = "naviproducts/with-items";
        public const string NaviProductUpdateWithItems      = "naviproducts/{0}/with-items";
        public const string NaviProductDeleteWithItems      = "naviproducts/{0}/with-items";
        public const string NaviProductImportExcel   = "naviproducts/import-excel";

        // ── NaviItems ─────────────────────────────────────────────────
        public const string NaviItems                = "naviitems";
        public const string NaviItemById             = "naviitems/{0}";
        public const string NaviItemWithProducts     = "naviitems/{0}/products";
        public const string NaviItemByType           = "naviitems/type/{0}";
        public const string NaviItemSearch           = "naviitems/search";

        // ── NaviProductItems ──────────────────────────────────────────
        public const string NaviProductItems         = "naviproductitems";
        public const string NaviProductItemById      = "naviproductitems/{0}";
        public const string NaviProductItemsByProduct = "naviproductitems/product/{0}";
        public const string NaviProductItemsByItem   = "naviproductitems/item/{0}";
        public const string NaviProductItemExists    = "naviproductitems/exists";

        // ── NaviHistory ───────────────────────────────────────────────
        public const string NaviHistory              = "navihistory";
        public const string NaviHistoryById          = "navihistory/{0}";
        public const string NaviHistoryByCodeNV      = "navihistory/nv/{0}";
        public const string NaviHistoryByProductItem = "navihistory/productitem/{0}";
        public const string NaviHistoryByPO          = "navihistory/po/{0}";

        // ── Legacy (giữ lại để không break code cũ) ───────────────────
        public const string Processes                = "processes";
        public const string ProcessById              = "processes/{0}";
        public const string ProcessSteps             = "processes/{0}/steps";
        public const string Steps                    = "steps";
        public const string StepById                 = "steps/{0}";
        public const string StepExecute              = "steps/{0}/execute";
        public const string StepUpdateStatus         = "steps/{0}/status";
        public const string ProcessStatistics        = "processes/statistics";
    }
}
