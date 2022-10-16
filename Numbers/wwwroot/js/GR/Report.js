
$(document).ready(function () {
    $('#divStartDate').hide();
    $('#divEndDate').hide();
    $('#divSuppliers').hide();
    $('#divConstruction').hide();
    $('#divContractNo').hide();
    $('#divLevels').hide();
    $('#divLocation').hide();
    $('#divBrand').hide()
    //divMonths
    $('#divProductionOrderNo').hide()
    $('#divIssueNo').hide()
    $('#divQtyLevels').hide();
     $('#Construction').select2();
   $('#ContractNo').select2();
    $('#Suppliers').select2();
   // $('#Construction').select2();
   //$('#ContractNo').select2();
   // $('#Suppliers').select2();
    $('#IssueNo').select2();
    $('#ProductionOrderNo').select2();
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
        

        case "WeavingContract":
            //Hide code
            $('#FormNo').text("PO #");
            $('#divCategory').hide();
            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divItems').hide();
            $('#divConstruction').show();
            $('#divContractNo').show();
            $('#divLevels').hide();
            $('#divSuppliers').show();
            $('#divLocation').hide();
            $('#divProductionOrderNo').hide()
            $('#divIssueNo').hide()
            $('#divBrand').hide()
            $('#divQtyLevels').hide();
        
            break;
        case "GreigeMending":
            //Hide code
            $('#FormNo').text("PO #");
            $('#divCategory').hide();
            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divConstruction').show();
            $('#divContractNo').show();
            $('#divLevels').hide();
            $('#divSuppliers').show();
            $('#divLocation').hide();
            $('#divProductionOrderNo').hide()
            $('#divIssueNo').hide()
            $('#divBrand').hide()
            $('#divQtyLevels').hide();
           
            break;

        case "GreigeRejection":
            //Hide code
            $('#FormNo').text("PO #");
            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divConstruction').show();
            $('#divContractNo').show();
            $('#divLocation').hide();
            $('#divSuppliers').show();
            $('#divLevels').hide();
            $('#divProductionOrderNo').hide()
            $('#divIssueNo').hide()
            $('#divBrand').hide()
            $('#divQtyLevels').hide();
        
            break;
        case "KachiParchiIGP":
            //Hide code
            $('#FormNo').text("PO #");
            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divConstruction').hide();
            $('#divContractNo').show();
            $('#divLevels').hide();
            $('#divSuppliers').hide();
            $('#divLocation').hide();
            $('#divProductionOrderNo').hide()
            $('#divIssueNo').hide()
            $('#divBrand').hide()
            $('#divQtyLevels').show();
            break;
        case "PakiParchiGRN":
            //Hide code
            $('#FormNo').text("PO #");
            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divProductionOrderNo').hide()
            $('#divIssueNo').hide()
            $('#divConstruction').show();
            $('#divContractNo').hide();
            $('#divLocation').hide();
            $('#divLevels').show();
            $('#divSuppliers').hide();
            $('#divBrand').hide()
            $('#divQtyLevels').hide();
            break;
        case "GreigeIssue":
            //Hide code
            $('#FormNo').text("PO #");
            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divProductionOrderNo').show()
            $('#divIssueNo').show()
            $('#divBrand').show()

            $('#divConstruction').hide();
            $('#divContractNo').hide();
            $('#divLocation').hide();
            $('#divLevels').hide();
            $('#divSuppliers').hide();
            $('#divQtyLevels').hide();
            break;
        case "UnliftedGreige":

            $('#divBrand').show()
            $('#divProductionOrderNo').show()
            $('#divConstruction').show();
          
            $('#divQtyLevels').show();
            $('#divStartDate').show();

            //Hide code
            $('#FormNo').text("PO #");
            $('#divEndDate').hide();
            $('#divIssueNo').hide()
            $('#divContractNo').hide();
            $('#divLocation').hide();
            $('#divSuppliers').hide();
            $('#divLevels').hide();
            break;
        case "ContractStatus":
            //Hide code
            $('#FormNo').text("PO #");
            $('#divCategory').hide();
            $('#divStartDate').show();
            $('#divEndDate').show();
            $('#divItems').hide();
            $('#divConstruction').show();
            $('#divContractNo').show();
            $('#divLevels').hide();
            $('#divSuppliers').show();
            $('#divLocation').hide();
            $('#divProductionOrderNo').hide()
            $('#divIssueNo').hide()
            $('#divBrand').hide()
            $('#divQtyLevels').hide();

            break;
        default:
            debugger;
            // code block
            $('#divStartDate').hide();
            $('#divEndDate').hide();
            $('#divSuppliers').hide();
            $('#divConstruction').hide();
            $('#divContractNo').hide();
            $('#divLevels').hide();
            $('#divQtyLevels').hide();
            $('#divLocation').hide();
            $('#divProductionOrderNo').hide()
            $('#divIssueNo').hide()
            $('#divBrand').hide()
            
            break;
    }
}





 