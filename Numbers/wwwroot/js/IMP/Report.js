
$(document).ready(function () {
    $('#divStartDate').hide();
    $('#divEndDate').hide();
    $('#divSuppliers').hide();
    $('#divItems').hide();
    $('#divCategory').hide();
    $('#divManufacturers').hide();
    $('#divCategory4').hide();
    $('#divDepartments').hide();
    $('#divPrNo').hide();
    $('#divRefrenceNo').hide();
    $('#divApproveRejected').hide();
    $('#divCity').hide();
    $('#divCountry').hide();
    $('#divRegistered').hide();
    $('#divCostCenters').hide();
    $('#divType').hide();
    $('#divReportType').hide();
    $('#divGRNOrPayment').hide();
    $('#divInsuranceNo').hide();
    $('#divMonths').hide();
    //divMonths
    $('#Item').select2();
    $('#Suppliers').select2();
    $('#Manufacturers').select2();
    $('#CostCenters').select2();
    $('#Departments').select2();
    $('#Categories').select2();
    $('#Categories4').select2();
    $('#Cities').select2();
    $('#Countries').select2();
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
            break;
        case "UserReportTest":
            //Hide Area
           
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
            $('#divType').hide();
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
            break;
        case "PurchaseRequisitionStatusSummary":
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
            break;
        case "ComparativeStatement":
            //Hide code
            $('#FormNo').text("CS #");
            $('#refNo').text("PR #");
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
            $('#divSuppliers').show();
            $('#divCity').hide();
            $('#divRegistered').hide();
            $('#divType').hide();
            $('#divReportType').hide();
            $('#divGRNOrPayment').hide();
            $('#divInsuranceNo').hide();
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
            break;
        case "InvoiceFeedingSummary":
            //Hide Area
            $('#FormNo').text("PO #");
            $('#refNo').text("GRN #");
            $('#refNo').show();
            $('#divCategory').hide();
            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divItems').hide();
            $('#divDepartments').hide();
            $('#divPrNo').show();
            $('#divRefrenceNo').show();
            $('#divApproveRejected').hide();
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
            break;
        case "POVendorWiseTaxDeduction":
            //Hide Area
            $('#FormNo').text("PO #");
            $('#refNo').text("GRN #");
            $('#refNo').hide();
            $('#divCategory').hide();
            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divItems').hide();
            $('#divDepartments').hide();
            $('#divPrNo').show();
            $('#divRefrenceNo').hide();
            $('#divApproveRejected').show();
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
            break;
        case "PurchaseComparisonReport(MonthWise)":
            //Hide Area
            $('#divStartDate').show();
            $('#divMonths').show();
            $('#divEndDate').hide();
            $('#divSuppliers').hide();
            $('#divItems').show();
            $('#divCategory').hide();
            $('#divManufacturers').hide();
            $('#divCategory4').show();
            $('#divDepartments').hide();
            $('#divPrNo').hide();
            $('#divRefrenceNo').hide();
            $('#divApproveRejected').hide();
            $('#divCity').hide();
            $('#divCountry').hide();
            $('#divRegistered').hide();
            $('#divCostCenters').hide();
            $('#divType').hide();
            $('#divReportType').hide();
            $('#divGRNOrPayment').hide();
            $('#divInsuranceNo').hide();
            break;
        case "VendorListReport":
            //Hide Area
            $('#divStartDate').hide();
            $('#divEndDate').hide();
            $('#divSuppliers').show();
            $('#divItems').hide();
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
            $('#divCostCenters').hide();
            $('#divType').hide();
            $('#divReportType').hide();
            $('#divGRNOrPayment').hide();
            $('#divInsuranceNo').hide();
            break;
        case "LoanMaterial":
            //Hide code
            $('#FormNo').text("PO #");
            $('#divCategory').hide();
            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divItems').show();
            $('#divCategory').hide();
            $('#divDepartments').hide();
            $('#divPrNo').show();
            $('#divRefrenceNo').hide();
            $('#divApproveRejected').show();
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
            break;
        case "VendorAgingReportBasedonDueDate":
            //Hide code
            $('#FormNo').text("PO #");
            $('#divCategory').hide();
            $('#divStartDate').show();
            $('#divEndDate').hide();
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
            break;
        case "VendorAgingReportBasedonGRN":
            //Hide code
            $('#FormNo').text("PO #");
            $('#divCategory').hide();
            $('#divStartDate').show();
            $('#divEndDate').hide();
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
            break;
        case "RateTrend":
            //Hide code
            $('#FormNo').text("PO #");
            $('#divCategory').hide();
            $('#divStartDate').show();
            $('#divEndDate').hide();
            $('#divItems').hide();
            $('#divCategory').hide();
            $('#divDepartments').hide();
            $('#divPrNo').hide();
            $('#divRefrenceNo').hide();
            $('#divApproveRejected').hide();
            $('#divCostCenters').hide();
            // Show Area
            $('#divManufacturers').hide();
            $('#divSuppliers').hide();
            $('#divCity').hide();
            $('#divRegistered').hide();
            $('#divType').hide();
            $('#divReportType').hide();
            $('#divGRNOrPayment').hide();
            $('#divInsuranceNo').hide();
            break;

        case "LCLimitVsBusinessReport":
            //Hide code
            $('#FormNo').text("PO #");
            $('#divCategory').hide();
            $('#divStartDate').show();
            $('#divEndDate').hide();
            $('#divItems').hide();
            $('#divCategory').hide();
            $('#divDepartments').hide();
            $('#divPrNo').hide();
            $('#divRefrenceNo').hide();
            $('#divApproveRejected').hide();
            $('#divCostCenters').hide();
            // Show Area
            $('#divManufacturers').hide();
            $('#divSuppliers').hide();
            $('#divCity').hide();
            $('#divRegistered').hide();
            $('#divType').hide();
            $('#divReportType').hide();
            $('#divGRNOrPayment').hide();
            $('#divInsuranceNo').hide();
            break;
        default:
            debugger;
            // code block
            $('#divStartDate').hide();
            $('#divEndDate').hide();
            $('#divSuppliers').hide();
            $('#divItems').hide();
            $('#divCategory').hide();
            $('#divManufacturers').hide();
            $('#divCategory4').hide();
            $('#divDepartments').hide();
            $('#divPrNo').hide();
            $('#divRefrenceNo').hide();
            $('#divApproveRejected').hide();
            $('#divCity').hide();
            $('#divCountry').hide();
            $('#divRegistered').hide();
            $('#divCostCenters').hide();
            $('#divType').hide();
            $('#divReportType').hide();
            $('#divGRNOrPayment').hide();
            $('#divInsuranceNo').hide();
            $('#divMonths').hide();
            break;
    }
}





 