
$(document).ready(function () {
    $('#divStartDate').hide();
    $('#divEndDate').hide();
    $('#divSONo').hide();
    $('#divApproved').hide();
    $('#divProductType').hide();
    $('#divVehicalNo').hide();
    $('#divDCNo').hide();
    $('#divInvoiceNo').hide();
    $('#divItems').hide();
    $('#divCategories').hide();
    $('#divManufacturer').hide();
    $('#divCustomers').hide();
    $('#divSalePerson').hide();
    $('#divCustomerCategory').hide();
    $('#divCities').hide();
    $('#divLevels').hide();
    $('#divLevels2').hide();
    $('#divFourthLevel').hide();
    $('#divagents').hide();
    //ProductType
    $('#divSeason').hide();
    $('#divRecoveryCategory').hide();
    $('#divSecondItemCategory').hide();
    //$('#ReportTitle').select2();
    $('#ReportTitle').select2({}).on("change", function (e) {
        reportParams();
    });
    
});
$('#ReportTitle').change(function () {
    reportParams();
});

function reportParams() {
    var reportName = $('#ReportTitle').val();
    switch (reportName) {
        case "PurchaseLedgerItemWise":
            // Show Area
            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divSuppliers').show();
            $('#divItems').show();
            //Hide Area
            $('#divCategory').hide();
            $('#divManufacturers').hide();
            $('#divCategory').hide();
            $('#divDepartments').hide();
            $('#divPrNo').hide();
            $('#divRefrenceNo').hide();
            $('#divApproveRejected').hide();
            $('#divCity').hide();
            $('#divCountry').hide();
            $('#divRegistered').hide();
            $('#divType').hide();
            $('#divReportType').hide();
            $('#divGRNOrPayment').hide();
            $('#divInsuranceNo').hide();
            $('#divSeason').hide();
            $('#divagents').hide();
            $('#divFourthLevel').hide();
            $('#divRecoveryCategory').hide();
            $('#divSecondItemCategory').hide();
            $('#divSalePerson').hide();
            break;
        case "PurchaseJournal":
            //Hide Area
            $('#divCategory').show();
            $('#divManufacturers').show();
           // Show Area
            $('#divStartDate').hide();
            $('#divEndDate').hide();
            $('#divSuppliers').hide();
            $('#divItems').hide();
            $('#divCategory').hide();
            $('#divDepartments').hide();
            $('#divPrNo').hide();
            $('#divRefrenceNo').hide();
            $('#divApproveRejected').hide();
            $('#divCity').hide();
            $('#divCountry').hide();
            $('#divCostCenters').hide();
            $('#divRegistered').hide();
            $('#divType').hide();
            $('#divReportType').hide();
            $('#divGRNOrPayment').hide();
            $('#divInsuranceNo').hide();
            $('#divSeason').hide();
            $('#divagents').hide();
            $('#divFourthLevel').hide();
            $('#divRecoveryCategory').hide();
            $('#divSecondItemCategory').hide();
            $('#divSalePerson').hide();
            break;
        case "PurchaseRequisition":
                 //Hide Area
            $('#FormNo').text("PR #");
            $('#divCategory').show();
            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divItems').show();
            $('#divDepartments').show();
            $('#divPrNo').show();
            $('#divRefrenceNo').show();
            $('#divApproveRejected').show();
            $('#divCostCenters').show();
                // Show Area
            $('#divManufacturers').hide();
            $('#divSuppliers').hide();
            $('#divCity').hide();
            $('#divCountry').hide();
            $('#divRegistered').hide();
            $('#divSupplierType').hide();
            $('#divReportType').hide();
            $('#divGRNOrPayment').hide();
            $('#divInsuranceNo').hide();
            $('#divSeason').hide();
            $('#divFourthLevel').hide();
            $('#divagents').hide();
            $('#divRecoveryCategory').hide();
            $('#divSecondItemCategory').hide();
            $('#divSalePerson').hide();
            break;
        case "Recovery":
            //Hide Area
            $('#divInvoiceNo').hide();
            $('#FormNo').text("PO #");
            $('#refNo').text("GRN #");
            $('#divCategory').hide();
            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divCategory').hide();
            $('#divReceiptNo').show();
            $('#divCategories').show();
            $('#divSalePerson').hide();
            $('#divItems').hide();
            $('#divDepartments').hide();
            $('#divPrNo').hide();
            $('#divRefrenceNo').hide();
            $('#divSONo').hide();
            $('#divDCNo').hide();
            $('#divApproved').hide();
            $('#divProductType').hide();
            // Show Area
            $('#divManufacturer').hide();
            $('#divCities').show();
            $('#divCustomers').show();
            $('#divCustomerCategory').hide();
            $('#divagents').hide();
            $('#divRecoveryCategory').hide();
            $('#divSecondItemCategory').hide();
            $('#divSalePerson').hide();
            break;
        case "PurchaseRequisitionStatus":
            //Hide Area
            $('#FormNo').text("PR #");
            $('#divCategory').hide();
            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divItems').hide();
            $('#divDepartments').show();
            $('#divPrNo').show();
            $('#divRefrenceNo').show();
            $('#divApproveRejected').hide();
            $('#divCostCenters').show();
            // Show Area
            $('#divManufacturers').hide();
            $('#divSuppliers').hide();
            $('#divCity').hide();
            $('#divCountry').hide();
            $('#divRegistered').hide();
            $('#divSupplierType').hide();
            $('#divReportType').hide();
            $('#divGRNOrPayment').hide();
            $('#divInsuranceNo').hide();
            $('#divSeason').hide();
            $('#divFourthLevel').hide();
            $('#divagents').hide();
            $('#divRecoveryCategory').hide();
            $('#divSecondItemCategory').hide();
            $('#divSalePerson').hide();
            break;
        case "PurchaseRequisitionStatusDetail":
            //Hide Area
            $('#FormNo').text("PR #");
            $('#divCategory').hide();
            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divItems').hide();
            $('#divDepartments').show();
            $('#divPrNo').show();
            $('#divRefrenceNo').show();
            $('#divApproveRejected').hide();
            $('#divCostCenters').show();
            // Show Area
            $('#divManufacturers').hide();
            $('#divSuppliers').hide();
            $('#divCity').hide();
            $('#divCountry').hide();
            $('#divRegistered').hide();
            $('#divSupplierType').hide();
            $('#divReportType').hide();
            $('#divGRNOrPayment').hide();
            $('#divInsuranceNo').hide();
            $('#divSeason').hide();
            $('#divFourthLevel').hide();
            $('#divagents').hide();
            $('#divRecoveryCategory').hide();
            $('#divSecondItemCategory').hide();
            $('#divSalePerson').hide();
            break;
        case "VendorListReport":
            //Hide code
            $('#divCategory').show();
            $('#divType').show();
            $('#divCity').show();
            $('#divCountry').show();
            $('#divRegistered').show();
            $('#divSuppliers').show();
            $('#divApproveRejected').show();
            $('#divType').show();
            // code block
            $('#divStartDate').hide();
            $('#divEndDate').hide();
            $('#divItems').hide();
            $('#divManufacturers').hide();
            $('#divDepartments').hide();
            $('#divPrNo').hide();
            $('#divRefrenceNo').hide();
            $('#divApproveRejected').show();
            $('#divCostCenters').hide();
            $('#divReportType').hide();
            $('#divGRNOrPayment').hide();
            $('#divInsuranceNo').hide();
            $('#divSeason').hide();
            $('#divFourthLevel').hide();
            $('#divagents').hide();
            $('#divRecoveryCategory').hide();
            $('#divSecondItemCategory').hide();
            $('#divSalePerson').hide();
            break;
        case "PurchaseOrderList":
            //Hide code
            $('#FormNo').text("PO #");
            $('#divCategory').hide();
            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divItems').show();
            $('#divCategory').hide();
            $('#divDepartments').show();
            $('#divPrNo').show();
            $('#divRefrenceNo').show();
            $('#divApproveRejected').show();
            $('#divCostCenters').show();
            // Show Area
            $('#divManufacturers').hide();
            $('#divSuppliers').show();
            $('#divCity').hide();
            $('#divRegistered').hide();
            $('#divType').hide();
            $('#divReportType').hide();
            $('#divGRNOrPayment').hide();
            $('#divInsuranceNo').hide();
            $('#divSeason').hide();
            $('#divFourthLevel').hide();
            $('#divagents').hide();
            $('#divRecoveryCategory').hide();
            $('#divSecondItemCategory').hide();
            $('#divSalePerson').hide();
            break;
        case "ComparativeStatement":
            //Hide code
            $('#FormNo').text("CS No");
            $('#divCategory').hide();
            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divItems').show();
            $('#divSuppliers').show();
            $('#divCategory').hide();
            $('#divDepartments').hide();
            $('#divPrNo').show();
            $('#divRefrenceNo').show();
            $('#divApproveRejected').show();
            $('#divCostCenters').hide();
            // Show Area
            $('#divManufacturers').hide();
            
            $('#divCity').hide();
            $('#divRegistered').hide();
            $('#divType').hide();
            $('#divReportType').hide();
            $('#divGRNOrPayment').hide();
            $('#divInsuranceNo').hide();
            $('#divSeason').hide();
            $('#divFourthLevel').hide();
            $('#divagents').hide();
            $('#divRecoveryCategory').hide();
            $('#divSecondItemCategory').hide();
            $('#divSalePerson').hide();
            break;
        case "PurchaseOrderVendorWise":
            //Hide code
            $('#FormNo').text("PO #");
            $('#divCategory').hide();
            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divItems').hide();
            $('#divCategory').hide();
            $('#divDepartments').hide();
            $('#divPrNo').show();
            $('#divRefrenceNo').hide();
            $('#divApproveRejected').hide();
            $('#divCostCenters').hide();
            // Show Area
            $('#divManufacturers').hide();
            $('#divSuppliers').show();
            $('#divCity').hide();
            $('#divRegistered').hide();
            $('#divType').hide();
            $('#divReportType').hide();
            $('#divGRNOrPayment').hide();
            $('#divInsuranceNo').hide();
            $('#divSeason').hide();
            $('#divagents').hide();
            $('#divFourthLevel').hide();
            $('#divRecoveryCategory').hide();
            $('#divSecondItemCategory').hide();
            $('#divSalePerson').hide();
            break;
        case "SupplierLedger":
            //Hide code
            $('#FormNo').text("PO #");
            $('#divCategory').hide();
            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divItems').hide();
            $('#divCategory').hide();
            $('#divDepartments').hide();
            $('#divPrNo').hide();
            $('#divRefrenceNo').hide();
            $('#divApproveRejected').hide();
            $('#divCostCenters').hide();
            // Show Area
            $('#divManufacturers').hide();
            $('#divSuppliers').show();
            $('#divCity').hide();
            $('#divRegistered').hide();
            $('#divType').hide();
            $('#divReportType').hide();
            $('#divGRNOrPayment').hide();
            $('#divInsuranceNo').hide();
            $('#divSeason').hide();
            $('#divagents').hide();
            $('#divFourthLevel').hide();
            $('#divRecoveryCategory').hide();
            $('#divSecondItemCategory').hide();
            $('#divSalePerson').hide();
            break;
        case "PurchaseOrderStatus":
            //Hide code
            $('#FormNo').text("PO #");
            $('#divCategory').hide();
            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divItems').show();
            $('#divCategory').hide();
            $('#divDepartments').show();
            $('#divPrNo').show();
            $('#divRefrenceNo').show();
            $('#divApproveRejected').hide();
            $('#divCostCenters').show();
            // Show Area
            $('#divManufacturers').hide();
            $('#divSuppliers').show();
            $('#divCity').hide();
            $('#divRegistered').show();
            $('#divType').hide();
            $('#divReportType').hide();
            $('#divGRNOrPayment').hide();
            $('#divInsuranceNo').hide();
            $('#divSeason').hide();
            $('#divagents').hide();
            $('#divFourthLevel').hide();
            $('#divRecoveryCategory').hide();
            $('#divSecondItemCategory').hide();
            $('#divSalePerson').hide();
            break;
        case "InsuranceExpenseReport":
            //Hide code
            $('#FormNo').text("PO #");
            $('#refNo').text("LC #");
            $('#divCategory').hide();
            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divItems').hide();
            $('#divCategory').hide();
            $('#divDepartments').hide();
            $('#divPrNo').show();
            $('#divRefrenceNo').show();
            $('#divInsuranceNo').show();
            $('#divApproveRejected').show();
            $('#divCostCenters').hide();
            // Show Area
            $('#divManufacturers').hide();
            $('#divSuppliers').hide();
            $('#divCity').hide();
            $('#divRegistered').hide();
            $('#divType').hide();
            $('#divReportType').hide();
            $('#divGRNOrPayment').hide();
            $('#divInsuranceNo').show();
            $('#divSeason').hide();
            $('#divagents').hide();
            $('#divFourthLevel').hide();
            $('#divRecoveryCategory').hide();
            $('#divSecondItemCategory').hide();
            $('#divSalePerson').hide();
            break;
        case "PurchaseReturn":
            //Hide code
            $('#FormNo').text("PR #");
            $('#divCategory').hide();
            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divItems').show();
            $('#divCategory').hide();
            $('#divDepartments').show();
            $('#divPrNo').show();
            $('#divRefrenceNo').show();
            $('#divApproveRejected').show();
            $('#divCostCenters').show();
            // Show Area
            $('#divManufacturers').hide();
            $('#divSuppliers').hide();
            $('#divCity').hide();
            $('#divRegistered').hide();
            $('#divType').hide();
            $('#divReportType').hide();
            $('#divGRNOrPayment').hide();
            $('#divInsuranceNo').hide();
            $('#divSeason').hide();
            $('#divFourthLevel').hide();
            $('#divagents').hide();
            $('#divRecoveryCategory').hide();
            $('#divSecondItemCategory').hide();
            $('#divSalePerson').hide();
            break;
        case "PurchaseComparisionMonthly":
            //Hide Area
            $('#FormNo').text("PO #");
            $('#divCategory').show();
            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divItems').show();
            $('#divDepartments').hide();
            $('#divPrNo').hide();
            $('#divRefrenceNo').hide();
            $('#divApproveRejected').hide();
            $('#divCostCenters').hide();
            $('#divType').hide();
            // Show Area
            $('#divManufacturers').hide();
            $('#divSuppliers').hide();
            $('#divCity').hide();
            $('#divCountry').hide();
            $('#divRegistered').hide();
            $('#divSupplierType').hide();
            $('#divReportType').hide();
            $('#divGRNOrPayment').hide();
            $('#divInsuranceNo').hide();
            $('#divSeason').hide();
            $('#divFourthLevel').hide();
            $('#divagents').hide();
            $('#divRecoveryCategory').hide();
            $('#divSecondItemCategory').hide();
            $('#divSalePerson').hide();
            break;

        case "VendorAging":
            //Hide Area
            $('#FormNo').text("PO #");
            $('#divReportType').show();
            $('#divGRNOrPayment').show();
            $('#divCategory').hide();
            $('#divStartDate').hide();
            $('#divEndDate').show();
            $('#divType').show();
            $('#divItems').hide();
            $('#divDepartments').hide();
            $('#divPrNo').hide();
            $('#divRefrenceNo').hide();
            $('#divApproveRejected').hide();
            $('#divCostCenters').hide();
            // Show Area
            $('#divManufacturers').hide();
            $('#divSuppliers').hide();
            $('#divCity').hide();
            $('#divCountry').hide();
            $('#divRegistered').hide();
            $('#divSupplierType').hide();
            $('#divInsuranceNo').hide();
            $('#divSeason').hide();
            $('#divFourthLevel').hide();
            $('#divagents').hide();
            $('#divRecoveryCategory').hide();
            $('#divSecondItemCategory').hide();
            $('#divSalePerson').hide();
            break;

        case "InvoicePosting":
            //Hide Area
            $('#FormNo').text("PO #");
            $('#refNo').text("GRN #");
            $('#divCategory').hide();
            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divItems').hide();
            $('#divDepartments').hide();
            $('#divPrNo').show();
            $('#divRefrenceNo').show();
            $('#divApproveRejected').show();
            $('#divCostCenters').hide();
            // Show Area
            $('#divManufacturers').hide();
            $('#divSuppliers').hide();
            $('#divCity').hide();
            $('#divCountry').hide();
            $('#divRegistered').hide();
            $('#divSupplierType').hide();
            $('#divReportType').hide();
            $('#divGRNOrPayment').hide();
            $('#divInsuranceNo').hide();
            $('#divSeason').hide();
            $('#divFourthLevel').hide();
            $('#divagents').hide();
            $('#divRecoveryCategory').hide();
            $('#divSecondItemCategory').hide();
            $('#divSalePerson').hide();
            break;

        case "POWisePendingLiability":
            //Hide Area
            $('#FormNo').text("PO #");
            $('#refNo').text("GRN #");
            $('#divCategory').hide();
            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divItems').hide();
            $('#divDepartments').hide();
            $('#divPrNo').show();
            $('#divRefrenceNo').hide();
            $('#divApproveRejected').hide();
            $('#divCostCenters').hide();
            // Show Area
            $('#divManufacturers').hide();
            $('#divSuppliers').show();
            $('#divCity').hide();
            $('#divCountry').hide();
            $('#divRegistered').hide();
            $('#divSupplierType').hide();
            $('#divReportType').hide();
            $('#divGRNOrPayment').hide();
            $('#divInsuranceNo').hide();
            $('#divSeason').hide();
            $('#divFourthLevel').hide();
            $('#divagents').hide();
            $('#divRecoveryCategory').hide();
            $('#divSecondItemCategory').hide();
            $('#divSalePerson').hide();
            break;
        case "SaleVsTarget":
            //Hide Area
            $('#divInvoiceNo').hide();
            $('#FormNo').text("PO #");
            $('#refNo').text("GRN #");
            $('#divCategory').hide();
            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divCategories').show();
            $('#divSalePerson').show();
            $('#divItems').hide();
            $('#divDepartments').hide();
            $('#divPrNo').hide();
            $('#divRefrenceNo').hide();
            $('#divSONo').hide();
            $('#divApproved').hide();
            $('#divProductType').hide();
            // Show Area
            $('#divManufacturer').hide();
            $('#divSeason').hide();
            $('#divCustomers').hide();
            $('#divCustomerCategory').hide();
            $('#divFourthLevel').hide();
            $('#divagents').hide();
            $('#divRecoveryCategory').hide();
            $('#divSecondItemCategory').hide();
            $('#divSalePerson').hide();
            break;
        case "CustomerReturnSummary":
            //Hide Area
            $('#divInvoiceNo').hide();
            $('#FormNo').text("Trans #");
            $('#refNo').text("GRN #");
            $('#divCategory').hide();
            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divCategories').show();
            $('#divSalePerson').show();
            $('#divItems').hide();
            $('#divDepartments').hide();
            $('#divPrNo').show();
            $('#divRefrenceNo').hide();
            $('#divSONo').hide();
            $('#divApproved').hide();
            $('#divProductType').hide();
            // Show Area
            $('#divManufacturer').hide();
            $('#divSeason').hide();
            $('#divCustomers').show();
            $('#divCustomerCategory').hide();
            $('#divFourthLevel').hide();
            $('#divCities').show();
            $('#divagents').hide();
        $('#divRecoveryCategory').hide();
            $('#divSecondItemCategory').hide();
            $('#divSalePerson').hide();
            break;
        case "CustomerReturnDetail":
            //Hide Area
            $('#divInvoiceNo').hide();
            $('#FormNo').text("Trans #");
            $('#refNo').text("GRN #");
            $('#divCategory').hide();
            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divCategories').show();
            $('#divSalePerson').show();
            $('#divItems').hide();
            $('#divDepartments').hide();
            $('#divPrNo').show();
            $('#divRefrenceNo').hide();
            $('#divSONo').hide();
            $('#divApproved').hide();
            $('#divProductType').hide();
            // Show Area
            $('#divManufacturer').hide();
            $('#divSeason').hide();
            $('#divCustomers').show();
            $('#divCustomerCategory').hide();
            $('#divFourthLevel').hide();
            $('#divCities').show();
            $('#divagents').hide();
        $('#divRecoveryCategory').hide();
            $('#divSecondItemCategory').hide();
            $('#divSalePerson').hide();
            break;
        case "SalesInvoiceDateWise":


            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divCategories').hide();
            $('#divSalePerson').hide();
            $('#divItems').hide();
            $('#divDepartments').hide();
            $('#divPrNo').hide();
            $('#divRefrenceNo').hide();
            $('#divSONo').hide();
            $('#divDCNo').hide();
            $('#divApproved').hide();
            $('#divProductType').hide();
            // Show Area
            $('#divManufacturer').hide();
            $('#divCities').hide();
            $('#divCustomers').hide();
            $('#divCustomerCategory').hide();
            $('#divInvoiceNo').hide();
            $('#FormNo').text("PO #");
            $('#refNo').text("GRN #");
            $('#divCategory').hide();
            $('#divSeason').hide();
            $('#divFourthLevel').hide();
            $('#divagents').hide();
        $('#divRecoveryCategory').hide();
            $('#divSecondItemCategory').hide();
            $('#divSalePerson').hide();
            break;
        case "DispatchReport":
            //Hide Area
            $('#divInvoiceNo').hide();
            $('#FormNo').text("PO #");
            $('#refNo').text("GRN #");
            $('#divCategory').hide();
            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divCategories').show();
            $('#divSalePerson').hide();
            $('#divItems').hide();
            $('#divDepartments').hide();
            $('#divPrNo').hide();
            $('#divRefrenceNo').hide();
            $('#divSONo').show();
            $('#divDCNo').show();
            $('#divApproved').show();
            $('#divProductType').hide();
            // Show Area divLevels
            $('#divManufacturer').hide();
            $('#divLevels').show();
            $('#divCities').show();
            $('#divCustomers').show();
            $('#divCustomerCategory').hide();
            $('#divSeason').hide();
            $('#divFourthLevel').hide();
            $('#divagents').hide();
        $('#divRecoveryCategory').hide();
            $('#divSecondItemCategory').hide();
            $('#divSalePerson').hide();
            break;
        case "SaleOrderReport":
            //Hide Area
            $('#divInvoiceNo').hide();
            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divCategories').show();
            $('#divSalePerson').show();
            $('#divCities').show();
            $('#divCustomers').show();
            $('#divSONo').show();
            $('#divDCNo').hide();
            $('#divCustomerCategory').hide();
            $('#divManufacturer').hide();
            $('#divCities').hide();
            $('#divSeason').hide();
            $('#divFourthLevel').hide();
            $('#divagents').hide();
        $('#divRecoveryCategory').hide();
            $('#divSecondItemCategory').hide();
            $('#divSalePerson').hide();
            break;
        case "SaleOrderStatus":
            //Hide Area
            $('#divInvoiceNo').hide();
            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divCategories').show();
            $('#divSalePerson').show();
            $('#divCities').show();
            $('#divCustomers').show();
            $('#divSONo').show();
            $('#divDCNo').hide();
            $('#divCustomerCategory').hide();
            $('#divManufacturer').hide();
            $('#divCities').show();
            $('#divSeason').hide();
            $('#divFourthLevel').hide();
            $('#divagents').hide();
        $('#divRecoveryCategory').hide();
            $('#divSecondItemCategory').hide();
            $('#divSalePerson').hide();
            break;
        case "SaleOrderStatusQtyWise":
            //Hide Area
            $('#divInvoiceNo').hide();
            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divCategories').show();
            $('#divSalePerson').show();
            $('#divCities').show();
            $('#divCustomers').show();
            $('#divSONo').show();
            $('#divDCNo').hide();
            $('#divCustomerCategory').hide();
            $('#divManufacturer').hide();
            $('#divCities').show();
            $('#divSeason').hide();
            $('#divFourthLevel').hide();
            $('#divagents').hide();
        $('#divRecoveryCategory').hide();
            $('#divSecondItemCategory').hide();
            $('#divSalePerson').hide();
            break;
        case "InvoiceWiseSaleSummary":
            //Hide Area
            
            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divInvoiceNo').show();
            $('#divCategories').show();
            $('#divSalePerson').show();
            $('#divCities').show();
            $('#divCustomers').show();
            $('#divSONo').show();
            $('#divDCNo').hide();
            $('#divCustomerCategory').hide();
            $('#divManufacturer').hide();
            $('#divCities').show();
            $('#divSeason').hide();
            $('#divFourthLevel').hide();
            $('#divagents').hide();
        $('#divRecoveryCategory').hide();
            $('#divSecondItemCategory').hide();
            $('#divSalePerson').hide();
            break;
        case "MonthlySaleTrend":
            //Hide Area

            $('#divStartDate').show();
            $('#divEndDate').hide();
            $('#divInvoiceNo').show();
            $('#divCategories').show();
            $('#divSalePerson').show();
            $('#divCities').show();
            $('#divCustomers').show();
            $('#divInvoiceNo').hide();
            $('#divSONo').hide();
            $('#divDCNo').hide();
            $('#divCustomerCategory').hide();
            $('#divManufacturer').hide();
            $('#divCities').show();
            $('#divSeason').hide();
            $('#divFourthLevel').hide();
            $('#divLevels2').show();
            $('#divagents').hide();
            $('#divLevels').show();
            $('#divProductType').show();
        $('#divRecoveryCategory').hide();
            $('#divSecondItemCategory').hide();
            $('#divSalePerson').hide();
            
            break;
        case "ReturnStatus":
            //Hide Area
            $('#divInvoiceNo').hide();
            $('#FormNo').text("PO #");
            $('#refNo').text("GRN #");
            $('#divCategory').hide();
            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divCategory').show();
            $('#divCategories').hide();
            $('#divSalePerson').show();
            $('#divItems').hide();
            $('#divDepartments').hide();
            $('#divPrNo').hide();
            $('#divRefrenceNo').hide();
            $('#divSONo').hide();
            $('#divDCNo').hide();
            $('#divApproved').hide();
            $('#divProductType').hide();
            // Show Area
            $('#divManufacturer').hide();
            $('#divCities').show();
            $('#divCustomers').show();
            $('#divCustomerCategory').hide();
            $('#divagents').hide();
        $('#divRecoveryCategory').hide();
            $('#divSecondItemCategory').hide();
            $('#divSalePerson').hide();
            break;
        case "PendingInvoices":


            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divCategories').hide();
            $('#divSalePerson').hide();
            $('#divItems').hide();
            $('#divDepartments').hide();
            $('#divPrNo').hide();
            $('#divRefrenceNo').hide();
            $('#divSONo').hide();
            $('#divDCNo').show();
            $('#divApproved').hide();
            $('#divProductType').hide();
            // Show Area
            $('#divManufacturer').hide();
            $('#divCities').show();
            $('#divCustomers').show();
            $('#divCustomerCategory').hide();
            $('#divInvoiceNo').hide();
            $('#FormNo').text("PO #");
            $('#refNo').text("GRN #");
            $('#divCategory').show();
            $('#divagents').hide();
        $('#divRecoveryCategory').hide();
            $('#divSecondItemCategory').hide();
            $('#divSalePerson').hide();

            break;

        case "RateAndDiscountMatrix":
            //Hide Area

            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divProductType').show();
            $('#divSeason').show();
            $('#divInvoiceNo').hide();
            $('#divCategories').show();
            $('#divSalePerson').hide();
            $('#divCities').hide();
            $('#divCustomers').hide();
            $('#divSONo').hide();
            $('#divDCNo').hide();
            $('#divCustomerCategory').hide();
            $('#divManufacturer').hide();
            $('#divCities').hide();
            $('#divFourthLevel').hide();
            $('#divagents').hide();
        $('#divRecoveryCategory').hide();
            $('#divSecondItemCategory').hide();
            $('#divSalePerson').hide();
            
            break;
        case "SaleSummary":
            //Hide Area

            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divInvoiceNo').hide();
            $('#divCategories').show();
            $('#divSalePerson').show();
            $('#divCities').show();
            $('#divCustomers').show();
            $('#divSONo').hide();
            $('#divDCNo').hide();
            $('#divCustomerCategory').hide();
            $('#divManufacturer').hide();
            $('#divCities').show();
            $('#divLevels').show();
            $('#divSeason').hide();
            $('#divFourthLevel').hide();
            $('#divagents').hide();
        $('#divRecoveryCategory').hide();
            $('#divSecondItemCategory').hide();
            $('#divSalePerson').hide();
            break;
        case "SalesInvoiceDateWise":

           
            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divCategories').hide();
            $('#divSalePerson').hide();
            $('#divItems').hide();
            $('#divDepartments').hide();
            $('#divPrNo').hide();
            $('#divRefrenceNo').hide();
            $('#divSONo').hide();
            $('#divDCNo').hide();
            $('#divApproved').hide();
            $('#divProductType').hide();
            // Show Area
            $('#divManufacturer').hide();
            $('#divCities').hide();
            $('#divCustomers').hide();
            $('#divCustomerCategory').hide();
            $('#divInvoiceNo').hide();
            $('#FormNo').text("PO #");
            $('#refNo').text("GRN #");
            $('#divCategory').hide();
            $('#divSeason').hide();
            $('#divFourthLevel').hide();
            $('#divagents').hide();
        $('#divRecoveryCategory').hide();
            $('#divSecondItemCategory').hide();
            $('#divSalePerson').hide();
            break;
        case "CustomerList":
            $('#divStartDate').hide();
            $('#divEndDate').hide();
            $('#divSONo').hide();
            $('#divApproved').hide();
            $('#divProductType').show();
            $('#divVehicalNo').hide();
            $('#divDCNo').hide();
            $('#divInvoiceNo').hide();
            $('#divItems').hide();
            $('#divCategories').show();
            $('#divManufacturer').hide();
            $('#divCustomers').show();
            $('#divSalePerson').show();
            $('#divCustomerCategory').show();
            $('#divCities').show();
            $('#divSeason').hide();
            $('#divFourthLevel').hide();
            $('#divagents').hide();
        $('#divRecoveryCategory').hide();
            $('#divSecondItemCategory').hide();
            $('#divSalePerson').hide();
            break;
        case "CustomerCreditLimitStatus":
            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divSONo').hide();
            $('#divApproved').hide();
            $('#divProductType').show();
            $('#divVehicalNo').hide();
            $('#divDCNo').hide();
            $('#divInvoiceNo').hide();
            $('#divItems').hide();
            $('#divCategories').show();
            $('#divManufacturer').hide();
            $('#divCustomers').show();
            $('#divSalePerson').show();
            $('#divCustomerCategory').show();
            $('#divCities').show();
            $('#divSeason').hide();
            $('#divFourthLevel').hide();
            $('#divagents').hide();
        $('#divRecoveryCategory').hide();
            $('#divSecondItemCategory').hide();
            $('#divSalePerson').hide();
            break;
        case "CustomerLedger":
            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divSONo').hide();
            $('#divApproved').hide();
            $('#divProductType').hide();
            $('#divVehicalNo').hide();
            $('#divDCNo').hide();
            $('#divInvoiceNo').hide();
            $('#divItems').hide();
            $('#divCategories').hide();
            $('#divManufacturer').hide();
            $('#divCustomers').show();
            $('#divSalePerson').hide();
            $('#divCustomerCategory').hide();
            $('#divCities').hide();
            $('#divSeason').hide();
            $('#divFourthLevel').hide();
            $('#divagents').hide();
        $('#divRecoveryCategory').hide();
            $('#divSecondItemCategory').hide();
            $('#divSalePerson').hide();
            break;
        case "SalePersonTarget":
            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divSONo').hide();
            $('#divApproved').hide();
            $('#divProductType').hide();
            $('#divVehicalNo').hide();
            $('#divDCNo').hide();
            $('#divInvoiceNo').hide();
            $('#divItems').hide();
            $('#divCategories').hide();
            $('#divManufacturer').hide();
            $('#divCustomers').hide();
            $('#divSalePerson').show();
            $('#divCustomerCategory').hide();
            $('#divCities').hide();
            $('#divSeason').hide();
            $('#divFourthLevel').hide();
            $('#divagents').hide();
        $('#divRecoveryCategory').hide();
            $('#divSecondItemCategory').hide();
            $('#divSalePerson').hide();
            break;
        case "SaleAnalysisLFL":
            //Hide Area   divLevels
            $('#FormNo').text("PO #");

            $('#divInvoiceNo').hide();
            $('#divStartDate').show();
            $('#divEndDate').hide();
            $('#divLevels').show();
            $('#divLevels2').show();
            $('#divCategories').hide();
            $('#divSalePerson').hide();
            $('#divCities').hide();
            $('#divCustomers').hide();
            $('#divSONo').hide();
            $('#divDCNo').hide();
            $('#divCustomerCategory').hide();
            $('#divManufacturer').hide();
            $('#divCities').hide();
            $('#divSeason').hide();
            $('#divFourthLevel').hide();
            $('#divagents').hide();
            $('#divRecoveryCategory').hide();
            $('#divSecondItemCategory').hide();
            $('#divSalePerson').hide();
            break;
        case "SaleDetailReport":
            //Hide Area   divLevels
            $('#FormNo').text("PO #");

            $('#divInvoiceNo').hide();
            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divLevels').hide();
            $('#divLevels2').hide();
            $('#divCategories').show();
            $('#divSalePerson').show();
            $('#divCities').show();
            $('#divCustomers').show();
            $('#divSONo').hide();
            $('#divDCNo').hide();
            $('#divCustomerCategory').hide();
            $('#divManufacturer').hide();
            $('#divSeason').hide();
            $('#divFourthLevel').hide();
            $('#divagents').hide();
        $('#divRecoveryCategory').hide();
            $('#divSecondItemCategory').hide();
            $('#divSalePerson').hide();
            break;
        case "StockVsSale":
            //Hide Area   divLevels
            $('#FormNo').text("PO #");

            $('#divInvoiceNo').hide();
            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divFourthLevel').show();
            $('#divLevels').show();
            $('#divLevels2').hide();
            $('#divCategories').show();
            $('#divSalePerson').show();
            $('#divCities').hide();
            $('#divCustomers').hide();
            $('#divSONo').hide();
            $('#divDCNo').hide();
            $('#divCustomerCategory').hide();
            $('#divManufacturer').hide();
            $('#divSeason').hide();
            $('#divagents').hide();
        $('#divRecoveryCategory').hide();
            $('#divSecondItemCategory').hide();
            $('#divSalePerson').hide();
            break;
        case "CustomerAgingReport":
            //Hide Area   divLevels
            $('#FormNo').text("PO #");

            $('#divInvoiceNo').hide();
            $('#divStartDate').show();
            $('#divEndDate').hide();
            $('#divFourthLevel').hide();
            $('#divLevels').hide();
            $('#divLevels2').hide();
            $('#divCategories').hide();
            $('#divSalePerson').hide();
            $('#divCities').hide();
            $('#divCustomers').show();
            $('#divSONo').hide();
            $('#divDCNo').hide();
            $('#divCustomerCategory').hide();
            $('#divManufacturer').hide();
            $('#divSeason').hide();
            $('#divagents').hide();
        $('#divRecoveryCategory').hide();
            $('#divSecondItemCategory').hide();
            $('#divSalePerson').hide();
            break;
        case "AgentWiseCommissione":
            debugger;
            //Hide Area
            //$('#divStartDate').show();
            //$('#divEndDate').show();
            $('#divInvoiceNo').hide();
            //$('#FormNo').text("PO #");
            //$('#refNo').text("GRN #");
            $('#divCategory').hide();
            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divCategory').hide();
            $('#divCategories').show();
            $('#divSalePerson').hide();
            $('#divItems').hide();
            $('#divDepartments').hide();
            $('#divPrNo').hide();
            $('#divRefrenceNo').hide();
            $('#divSONo').hide();
            $('#divDCNo').hide();
            $('#divApproved').hide();
            $('#divProductType').hide();
            // Show Area
            $('#divManufacturer').hide();
            $('#divCities').show();
            $('#divCustomers').hide();
            $('#divCustomerCategory').hide();
            $('#divagents').show();
        $('#divRecoveryCategory').hide();
            $('#divSecondItemCategory').hide();
            $('#divSalePerson').hide();
            break;

        case "PartyOverDueDaysCategoryWise":
            //Hide Area   divLevels
            $('#divInvoiceNo').hide();
            $('#divStartDate').show();
            $('#divEndDate').hide();
            $('#divFourthLevel').hide();
            $('#divLevels').hide();
            $('#divLevels2').hide();
            $('#divCategories').hide();
            $('#divSalePerson').hide();
            $('#divCities').show();
            $('#divCustomers').show();
            $('#divSONo').hide();
            $('#divDCNo').hide();
            $('#divCustomerCategory').hide();
            $('#divManufacturer').hide();
            $('#divSeason').hide();
            $('#divagents').hide();
            $('#divRecoveryCategory').show();
            $('#divSecondItemCategory').show();
            $('#divSalePerson').show();
            break;

        case "CustomerWiseOGP":
            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divSONo').hide();
            $('#divApproved').hide();
            $('#divProductType').hide();
            $('#divVehicalNo').show();
            $('#divDCNo').hide();
            $('#divInvoiceNo').hide();
            $('#divItems').hide();
            $('#divCategories').hide();
            $('#divManufacturer').hide();
            $('#divCustomers').hide();
            $('#divSalePerson').hide();
            $('#divCustomerCategory').hide();
            $('#divCities').hide();
            $('#divSeason').hide();
            $('#divFourthLevel').hide();
            $('#divagents').hide();
            $('#divVehicalNo').hide();
            break;


        default:

            debugger;
            $('#divStartDate').hide();
            $('#divEndDate').hide();
            $('#divSONo').hide();
            $('#divApproved').hide();
            $('#divProductType').hide();
            $('#divVehicalNo').hide();
            $('#divDCNo').hide();
            $('#divInvoiceNo').hide();
            $('#divItems').hide();
            $('#divCategories').hide();
            $('#divManufacturer').hide();
            $('#divCustomers').hide();
            $('#divSalePerson').hide();
            $('#divCustomerCategory').hide();
            $('#divCities').hide();
            $('#divLevels').hide();
            $('#divLevels2').hide();
            $('#divSeason').hide();
            $('#divFourthLevel').hide();
            $('#divagents').hide();
            $('#divRecoveryCategory').hide();
            $('#divSecondItemCategory').hide();
            $('#divSalePerson').hide();
            break;
    }
}





