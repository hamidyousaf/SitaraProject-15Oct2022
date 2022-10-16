
$(document).ready(function () {
    $('#divStartDate').hide();
    $('#divEndDate').hide();
    $('#divissueno').hide();
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
    $('#divIGP').hide();
    $('#divType').hide();
    $('#divReportType').hide();
    $('#divGRNOrPayment').hide();
    $('#divInsuranceNo').hide();
    $('#divMonths').hide();
    $('#divSeanalplanning').hide();
    $('#divYarnDo').hide();
    $('#divvendors').hide();
    $('#divIRNNo').hide();
    $('#divPONo').hide();
    $('#divissueno').hide();
    $('#divmontlyplanning').hide();
    
    //divMonths
    $('#Item').select2();
    $('#Suppliers').select2();
    $('#Manufacturers').select2();
    $('#CostCenters').select2();
    $('#Departments').select2();
    $('#Categories').select2();
    $('#Categories4').select2();
    $('#Cities').select2();
    $('#Contract').select2();
    $('#IssueNo').select2();
    $('#Weaver').select2();
    $('#SPNo').select2();
    $('#GreigeQuality').select2();
    $('#SeasonalGreigeQuality').select2();
    $('#Season').select2();
    $('#Countries').select2();
    $('#MpNo').select2();
    $('#Transdate').select2();
    $('#Planof').select2();
    $('#SpNo').select2();
    $('#SeasonalNo').select2();
    $('#PoNo').select2();
    $('#IrnNo').select2();
    $('#Vendors').select2();
    $('#IGP').select2();
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
            $('#divSeanalplanning').hide();
            $('#divCategory').hide();
            $('#divDepartments').hide();
            $('#divPrNo').hide();
            $('#divIGP').hide();
            $('#divRefrenceNo').hide();
            $('#divApproveRejected').hide();
            $('#divCity').hide();
            $('#divCountry').hide();
            $('#divRegistered').hide();
            $('#divType').hide();
            $('#divReportType').hide();
            $('#divGRNOrPayment').hide();
            $('#divissueno').hide();
            $('#divInsuranceNo').hide();
            $('#divYarnDo').hide();
            $('#divmontlyplanning').hide();
            $('#divissueno').hide();
            $('#divvendors').hide();
            $('#divIRNNo').hide();
            $('#divPONo').hide();
            break;
        case "PurchaseJournal":
            //Hide Area
            $('#divCategory').show();
            $('#divManufacturers').show();
            $('#divYarnDo').hide();
           // Show Area
            $('#divStartDate').hide();
            $('#divEndDate').hide();
            $('#divSuppliers').hide();
            $('#divItems').hide();
            $('#divCategory').hide();
            $('#divDepartments').hide();
            $('#divPrNo').hide();
            $('#divSeanalplanning').hide();
            $('#divRefrenceNo').hide();
            $('#divApproveRejected').hide();
            $('#divCity').hide();
            $('#divIGP').hide();
            $('#divCountry').hide();
            $('#divCostCenters').hide();
            $('#divRegistered').hide();
            $('#divType').hide();
            $('#divissueno').hide();
            $('#divReportType').hide();
            $('#divGRNOrPayment').hide();
            $('#divInsuranceNo').hide();
            $('#divmontlyplanning').hide();
            $('#divissueno').hide();
            $('#divvendors').hide();
            $('#divIRNNo').hide();
            $('#divPONo').hide();
            break;
        case "IRN":
            //Hide Area
            $('#divCategory').hide();
            $('#divManufacturers').hide();
            $('#divYarnDo').hide();
            // Show Area
            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divvendors').show();
            $('#divIRNNo').show();
            $('#divPONo').show();
            $('#divissueno').hide();
            $('#divSuppliers').hide();           
            $('#divItems').hide();
            $('#divCategory').hide();
            $('#divDepartments').hide();
            $('#divPrNo').hide();
            $('#divRefrenceNo').hide();
            $('#divApproveRejected').hide();
            $('#divIGP').hide();
            $('#divCity').hide();
            $('#divCountry').hide();
            $('#divCostCenters').hide();
            $('#divRegistered').hide();
            $('#divType').hide();
            $('#divissueno').hide();
            $('#divReportType').hide();
            $('#divGRNOrPayment').hide();
            $('#divInsuranceNo').hide();
            break;
        //case "UserReportTest":
        //    //Hide Area
           
        //    break;
        case "PurchaseRequisition":
                 //Hide Area
            $('#FormNo').text("PR #");
            $('#divCategory').show();
            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divItems').show();
            $('#divSeanalplanning').hide();
            $('#divDepartments').show();
            $('#divPrNo').show();
            $('#divRefrenceNo').show();
            $('#divApproveRejected').show();
            $('#divCostCenters').show();
                // Show Area
            $('#divManufacturers').hide();
            $('#divSuppliers').hide();
            $('#divCity').hide();
            $('#divIGP').hide();
            $('#divCountry').hide();
            $('#divissueno').hide();
            $('#divRegistered').hide();
            $('#divSupplierType').hide();
            $('#divReportType').hide();
            $('#divGRNOrPayment').hide();
            $('#divInsuranceNo').hide();
            $('#divType').hide();
            $('#divYarnDo').hide();
            $('#divmontlyplanning').hide();
            $('#divvendors').hide();
            $('#divissueno').hide();
            $('#divIRNNo').hide();
            $('#divPONo').hide();
            break;
        case "FreightLoadingUnloading":
            //Hide Area
            $('#FormNo').text("PR #");
            $('#divCategory').hide();
            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divItems').hide();
            $('#divissueno').hide();
            $('#divDepartments').hide();
            $('#divPrNo').hide();
            $('#divRefrenceNo').hide();
            $('#divApproveRejected').hide();
            $('#divIGP').hide();
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
            $('#divType').hide();
            $('#divYarnDo').hide();
            $('#divvendors').hide();
            $('#divIRNNo').hide();
            $('#divissueno').hide();
            $('#divPONo').show();
            $('#divIGP').show();
            break;
      
        case "PurchaseRequisitionStatus":
            //Hide Area
            $('#FormNo').text("PR #");
            $('#divSeanalplanning').hide();
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
            $('#divissueno').hide();
            $('#divInsuranceNo').hide();
            $('#divYarnDo').hide();
            $('#divmontlyplanning').hide();
            $('#divvendors').hide();
            $('#divIRNNo').hide();
            $('#divissueno').hide();
            $('#divPONo').hide();
            $('#divIGP').hide();
            break;
        case "PurchaseRequisitionStatusDetail":
            //Hide Area
            $('#FormNo').text("PR #");
            $('#divCategory').hide();
            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divSeanalplanning').hide();
            $('#divItems').hide();
            $('#divDepartments').show();
            $('#divPrNo').show();
            $('#divRefrenceNo').show();
            $('#divApproveRejected').hide();
            $('#divCostCenters').show();
            $('#divIGP').hide();
            // Show Area
            $('#divManufacturers').hide();
            $('#divSuppliers').hide();
            $('#divCity').hide();
            $('#divCountry').hide();
            $('#divRegistered').hide();
            $('#divissueno').hide();
            $('#divSupplierType').hide();
            $('#divReportType').hide();
            $('#divGRNOrPayment').hide();
            $('#divInsuranceNo').hide();
            $('#divYarnDo').hide();
            $('#divmontlyplanning').hide();
            $('#divissueno').hide();
            $('#divvendors').hide();
            $('#divIRNNo').hide();
            $('#divPONo').hide();
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
           
            $('#divissueno').hide();
            $('#divIGP').hide();
            // Show Area
            $('#divSeanalplanning').hide();
            $('#divManufacturers').hide();
            $('#divSuppliers').hide();
            $('#divCity').hide();
            $('#divCountry').hide();
            $('#divRegistered').hide();
            $('#divSupplierType').hide();
            $('#divReportType').hide();
            $('#divGRNOrPayment').hide();
            $('#divInsuranceNo').hide();
            $('#divYarnDo').hide();
            $('#divmontlyplanning').hide();
            $('#divvendors').hide();
            $('#divissueno').hide();
            $('#divIRNNo').hide();
            $('#divPONo').hide();
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
            $('#divYarnDo').hide();
            // code block
            $('#divIGP').hide();
            $('#divissueno').hide();
            $('#divStartDate').hide();
            $('#divSeanalplanning').hide();
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
            $('#divmontlyplanning').hide();
            $('#divvendors').hide();
            $('#divissueno').hide();
            $('#divIRNNo').hide();
            $('#divPONo').hide();
           
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
            $('#divSeanalplanning').hide();
            $('#divPrNo').show();
            $('#divRefrenceNo').show();
            $('#divApproveRejected').show();
            $('#divissueno').hide();
            $('#divCostCenters').show();
            $('#divYarnDo').hide();
            // Show Area
            $('#divManufacturers').hide();
            $('#divSuppliers').show();
            $('#divCity').hide();
            $('#divRegistered').hide();
            $('#divIGP').hide();
            $('#divType').hide();
            $('#divReportType').hide();
            $('#divGRNOrPayment').hide();
            $('#divInsuranceNo').hide();
            $('#divmontlyplanning').hide();
            $('#divvendors').hide();
            $('#divIRNNo').hide();
            $('#divissueno').hide();
            $('#divPONo').hide();
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
            $('#divSeanalplanning').hide();
            $('#divPrNo').show();
            $('#divRefrenceNo').show();
            $('#divissueno').hide();
            $('#divApproveRejected').show();
            $('#divCostCenters').hide();
            // Show Area
            $('#divManufacturers').hide();
            $('#divYarnDo').hide();
            
            $('#divCity').hide();
            $('#divIGP').hide();
            $('#divRegistered').hide();
            $('#divType').hide();
            $('#divReportType').hide();
            $('#divGRNOrPayment').hide();
            $('#divInsuranceNo').hide();
            $('#divmontlyplanning').hide();
            $('#divvendors').hide();
            $('#divissueno').hide();
            $('#divIRNNo').hide();
            $('#divPONo').hide();
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
            $('#divissueno').hide();
            $('#divPrNo').show();
            $('#divRefrenceNo').hide();
            $('#divApproveRejected').hide();
            $('#divCostCenters').hide();
            // Show Area
            $('#divSeanalplanning').hide();
            $('#divManufacturers').hide();
            $('#divSuppliers').show();
            $('#divCity').hide();
            $('#divRegistered').hide();
            $('#divType').hide();
            $('#divReportType').hide();
            $('#divGRNOrPayment').hide();
            $('#divInsuranceNo').hide();
            $('#divIGP').hide();
            $('#divYarnDo').hide();
            $('#divmontlyplanning').hide();
            $('#divvendors').hide();
            $('#divissueno').hide();
            $('#divIRNNo').hide();
            $('#divPONo').hide();
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
            $('#divSeanalplanning').hide();
            $('#divApproveRejected').hide();
            $('#divCostCenters').hide();
            // Show Area
            $('#divManufacturers').hide();
            $('#divissueno').hide();
            $('#divSuppliers').show();
            $('#divCity').hide();
            $('#divIGP').hide();
            $('#divRegistered').hide();
            $('#divType').hide();
            $('#divReportType').hide();
            $('#divGRNOrPayment').hide();
            $('#divInsuranceNo').hide();
            $('#divvendors').hide();
            $('#divIRNNo').hide();
            $('#divissueno').hide();
            $('#divPONo').hide();
            $('#divYarnDo').hide();
            $('#divmontlyplanning').hide();
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
            $('#divSeanalplanning').hide();
            $('#divApproveRejected').hide();
            $('#divCostCenters').show();
            // Show Area
            $('#divManufacturers').hide();
            $('#divissueno').hide();
            $('#divSuppliers').show();
            $('#divIGP').hide();
            $('#divCity').hide();
            $('#divRegistered').show();
            $('#divType').hide();
            $('#divReportType').hide();
            $('#divGRNOrPayment').hide();
            $('#divInsuranceNo').hide();
            $('#divvendors').hide();
            $('#divissueno').hide();
            $('#divIRNNo').hide();
            $('#divPONo').hide();
            $('#divYarnDo').hide();
            $('#divmontlyplanning').hide();
            break;
        case "InsuranceExpenseReport":
            //Hide code
            $('#FormNo').text("PO #");
            $('#refNo').text("LC #");
            $('#divCategory').hide();
            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divissueno').hide();
            $('#divItems').hide();
            $('#divCategory').hide();
            $('#divDepartments').hide();
            $('#divPrNo').show();
            $('#divRefrenceNo').show();
            $('#divSeanalplanning').hide();
            $('#divInsuranceNo').show();
            $('#divIGP').hide();
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
            $('#divYarnDo').hide();
            $('#divmontlyplanning').hide();
            $('#divissueno').hide();
            $('#divvendors').hide();
            $('#divIRNNo').hide();
            $('#divPONo').hide();
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
            $('#divSeanalplanning').hide();
            $('#divCostCenters').show();
            // Show Area
            $('#divIGP').hide();
            $('#divManufacturers').hide();
            $('#divSuppliers').show();
            $('#divCity').hide();
            $('#divRegistered').hide();
            $('#divissueno').hide();
            $('#divType').hide();
            $('#divReportType').hide();
            $('#divGRNOrPayment').hide();
            $('#divInsuranceNo').hide();
            $('#divYarnDo').hide();
            $('#divmontlyplanning').hide();
            $('#divvendors').hide();
            $('#divissueno').hide();
            $('#divIRNNo').hide();
            $('#divPONo').hide();
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
            $('#divIGP').hide();
            $('#divRefrenceNo').hide();
            $('#divApproveRejected').hide();
            $('#divCostCenters').hide();
            $('#divvendors').hide();
            $('#divIRNNo').hide();
            $('#divPONo').hide();
            $('#divType').hide();
            $('#divSeanalplanning').hide();
            // Show Area
            $('#divManufacturers').hide();
            $('#divSuppliers').hide();
            $('#divissueno').hide();
            $('#divCity').hide();
            $('#divCountry').hide();
            $('#divRegistered').hide();
            $('#divSupplierType').hide();
            $('#divReportType').hide();
            $('#divGRNOrPayment').hide();
            $('#divInsuranceNo').hide();
            $('#divissueno').hide();
            $('#divYarnDo').hide();
            $('#divmontlyplanning').hide();
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
            $('#divSeanalplanning').hide();
            $('#divRefrenceNo').hide();
            $('#divIGP').hide();
            $('#divApproveRejected').hide();
            $('#divCostCenters').hide();
            // Show Area
            $('#divManufacturers').hide();
            $('#divSuppliers').hide();
            $('#divCity').hide();
            $('#divCountry').hide();
            $('#divRegistered').hide();
            $('#divSupplierType').hide();
            $('#divissueno').hide();
            $('#divInsuranceNo').hide();
            $('#divvendors').hide();
            $('#divIRNNo').hide();
            $('#divPONo').hide();
            $('#divissueno').hide();
            $('#divYarnDo').hide();
            $('#divmontlyplanning').hide();
            break;

        case "InvoicePosting":
            //Hide Area
            $('#FormNo').text("PO #");
            $('#refNo').text("GRN #");
            $('#divCategory').hide();
            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divItems').hide();
            $('#divIGP').hide();
            $('#divDepartments').hide();
            $('#divPrNo').show();
            $('#divSeanalplanning').hide();
            $('#divRefrenceNo').show();
            $('#divApproveRejected').show();
            $('#divCostCenters').hide();
            // Show Area
            $('#divManufacturers').hide();
            $('#divissueno').hide();
            $('#divSuppliers').hide();
            $('#divCity').hide();
            $('#divCountry').hide();
            $('#divRegistered').hide();
            $('#divSupplierType').hide();
            $('#divReportType').hide();
            $('#divGRNOrPayment').hide();
            $('#divvendors').hide();
            $('#divIRNNo').hide();
            $('#divPONo').hide();
            $('#divissueno').hide();
            $('#divInsuranceNo').hide();
            $('#divYarnDo').hide();
            $('#divmontlyplanning').hide();
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
            $('#divSeanalplanning').hide();
            $('#divRefrenceNo').show();
            $('#divApproveRejected').hide();
            $('#divCostCenters').hide();
            $('#divIGP').hide();
            // Show Area
            $('#divissueno').hide();
            $('#divManufacturers').hide();
            $('#divSuppliers').hide();
            $('#divCity').hide();
            $('#divCountry').hide();
            $('#divRegistered').hide();
            $('#divSupplierType').hide();
            $('#divReportType').hide();
            $('#divGRNOrPayment').hide();
            $('#divInsuranceNo').hide();
            $('#divvendors').hide();
            $('#divIRNNo').hide();
            $('#divissueno').hide();
            $('#divPONo').hide();
            $('#divYarnDo').hide();
            $('#divmontlyplanning').hide();
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
            $('#divSeanalplanning').hide();
            $('#divIGP').hide();
            $('#divissueno').hide();
            $('#divApproveRejected').show();
            $('#divCostCenters').hide();
            // Show Area
            $('#divManufacturers').hide();
            $('#divSuppliers').show();
            $('#divCity').hide();
            $('#divCountry').hide();
            $('#divRegistered').hide();
            $('#divissueno').hide();
            $('#divSupplierType').hide();
            $('#divReportType').hide();
            $('#divvendors').hide();
            $('#divIRNNo').hide();
            $('#divPONo').hide();
            $('#divGRNOrPayment').hide();
            $('#divInsuranceNo').hide();
            $('#divYarnDo').hide();
            $('#divmontlyplanning').hide();
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
            $('#divSeanalplanning').hide();
            $('#divRefrenceNo').hide();
            $('#divApproveRejected').hide();
            $('#divIGP').hide();
            $('#divCostCenters').hide();
            // Show Area
            $('#divManufacturers').hide();
            $('#divSuppliers').show();
            $('#divissueno').hide();
            $('#divCity').hide();
            $('#divCountry').hide();
            $('#divissueno').hide();
            $('#divRegistered').hide();
            $('#divSupplierType').hide();
            $('#divReportType').hide();
            $('#divGRNOrPayment').hide();
            $('#divvendors').hide();
            $('#divIRNNo').hide();
            $('#divPONo').hide();
            $('#divInsuranceNo').hide();
            $('#divYarnDo').hide();
            $('#divmontlyplanning').hide();
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
            $('#divSeanalplanning').hide();
            $('#divDepartments').hide();
            $('#divPrNo').hide();
            $('#divRefrenceNo').hide();
            $('#divApproveRejected').hide();
            $('#divissueno').hide();
            $('#divCity').hide();
            $('#divIGP').hide();
            $('#divCountry').hide();
            $('#divRegistered').hide();
            $('#divCostCenters').hide();
            $('#divvendors').hide();
            $('#divIRNNo').hide();
            $('#divissueno').hide();
            $('#divPONo').hide();
            $('#divType').hide();
            $('#divReportType').hide();
            $('#divGRNOrPayment').hide();
            $('#divInsuranceNo').hide();
            $('#divYarnDo').hide();
            $('#divmontlyplanning').hide();
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
            $('#divvendors').hide();
            $('#divIRNNo').hide();
            $('#divPONo').hide();
            $('#divSeanalplanning').hide();
            $('#divApproveRejected').hide();
            $('#divCity').hide();
            $('#divissueno').hide();
            $('#divIGP').hide();
            $('#divCountry').hide();
            $('#divRegistered').hide();
            $('#divCostCenters').hide();
            $('#divissueno').hide();
            $('#divType').hide();
            $('#divReportType').hide();
            $('#divGRNOrPayment').hide();
            $('#divInsuranceNo').hide();
            $('#divYarnDo').hide();
            $('#divmontlyplanning').hide();
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
            $('#divvendors').hide();
            $('#divIRNNo').hide();
            $('#divPONo').hide();
            $('#divSeanalplanning').hide();
            $('#divPrNo').show();
            $('#divRefrenceNo').hide();
            $('#divIGP').hide();
            $('#divApproveRejected').show();
            $('#divCostCenters').hide();
            // Show Area
            $('#divManufacturers').hide();
            $('#divissueno').hide();
            $('#divSuppliers').show();
            $('#divCity').hide();
            $('#divissueno').hide();
            $('#divRegistered').hide();
            $('#divType').hide();
            $('#divReportType').hide();
            $('#divGRNOrPayment').hide();
            $('#divInsuranceNo').hide();
            $('#divYarnDo').hide();
            $('#divmontlyplanning').hide();
            break;
        case "VendorAgingReportBasedonDueDate":
            //Hide code
            $('#FormNo').text("PO #");
            $('#divCategory').hide();
            $('#divStartDate').show();
            $('#divEndDate').hide();
            $('#divItems').hide();
            $('#divCategory').hide();
            $('#divSeanalplanning').hide();
            $('#divDepartments').hide();
            $('#divPrNo').hide();
            $('#divIGP').hide();
            $('#divRefrenceNo').hide();
            $('#divApproveRejected').hide();
            $('#divCostCenters').hide();
            // Show Area
            $('#divManufacturers').hide();
            $('#divSuppliers').show();
            $('#divissueno').hide();
            $('#divvendors').hide();
            $('#divIRNNo').hide();
            $('#divPONo').hide();
            $('#divCity').hide();
            $('#divRegistered').hide();
            $('#divType').hide();
            $('#divReportType').hide();
            $('#divGRNOrPayment').hide();
            $('#divInsuranceNo').hide();
            $('#divYarnDo').hide();
            $('#divmontlyplanning').hide();
            break;
        case "VendorAgingReportBasedonGRN":
            //Hide code
            $('#FormNo').text("PO #");
            $('#divCategory').hide();
            $('#divStartDate').show();
            $('#divEndDate').hide();
            $('#divItems').hide();
            $('#divSeanalplanning').hide();
            $('#divCategory').hide();
            $('#divDepartments').hide();
            $('#divIGP').hide();
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
            $('#divissueno').hide();
            $('#divReportType').hide();
            $('#divissueno').hide();
            $('#divvendors').hide();
            $('#divIRNNo').hide();
            $('#divPONo').hide();
            $('#divGRNOrPayment').hide();
            $('#divInsuranceNo').hide();
            $('#divYarnDo').hide();
            $('#divmontlyplanning').hide();
            break;
        case "YarnDO":
            //Hide code
            $('#FormNo').text("PO #");
            $('#divSeanalplanning').hide();
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
            $('#divIGP').hide();
            // Show Area
            $('#divManufacturers').hide();
            $('#divissueno').show();
            $('#divSuppliers').hide();
            $('#divCity').hide();
            $('#divvendors').hide();
            $('#divIRNNo').hide();
            $('#divPONo').hide();
            $('#divRegistered').hide();
            $('#divType').hide();
            $('#divReportType').hide();
            $('#divGRNOrPayment').hide();
            $('#divInsuranceNo').hide();
            $('#divYarnDo').show();
            $('#divmontlyplanning').hide();
            break;
        case "MonthlyPlanning":
            //Hide code
            $('#FormNo').text("PO #");
            $('#divSeanalplanning').hide();
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
            $('#divSuppliers').hide();
            $('#divCity').hide();
            $('#divRegistered').hide();
            $('#divType').hide();
            $('#divReportType').hide();
            $('#divGRNOrPayment').hide();
            $('#divInsuranceNo').hide();
            $('#divYarnDo').hide();
            $('#divmontlyplanning').show();
            break;

        case "SeasonalPlanning":
            //Hide Area
            $('#divCategory').hide();
            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divItems').hide();
            $('#divDepartments').hide();
            $('#divSeanalplanning').show();
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
            $('#divYarnDo').hide();
            $('#divmontlyplanning').hide();
            break;



        default:
            debugger;
            // code block
            $('#divStartDate').hide();
            $('#divissueno').hide();
            $('#divSeanalplanning').hide();
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
            $('#divissueno').hide();
            $('#divCountry').hide();
            $('#divRegistered').hide();
            $('#divIGP').hide();
            $('#divCostCenters').hide();
            $('#divType').hide();
            $('#divReportType').hide();
            $('#divGRNOrPayment').hide();
            $('#divvendors').hide();
            $('#divIRNNo').hide();
            $('#divPONo').hide();
            $('#divInsuranceNo').hide();
            $('#divMonths').hide();
            $('#divYarnDo').hide();
            $('#divmontlyplanning').hide();
            break;
    }
}





 