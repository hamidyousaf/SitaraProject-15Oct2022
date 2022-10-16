var ids = [];
var grnItemQty = 0.0000;
$('.remove-item').click(function () {
    var elementId = this.id;
    console.log(elementId);
    var id = elementId.split('_');
    id = id[1];
    //ids to remove from database
    var itemId = $('#PurchaseItemId' + id).val();
    if (itemId != null && itemId > 0) {
        ids.push(itemId);
    }
    $('#IdsDeleted').text(ids);
    var digits = 2;
    var itemTotal = $('#Total_' + id).val();
    var lineDisc = $("#DiscountAmount_" + id).val();
    var lineSales = $("#SalesTaxAmount_" + id).val();
    var lineTotal = $("#LineTotal_" + id).val();
    //
    var pageTotal = ($('#Total').val().replace(/,/g, "") - itemTotal).toFixed(digits);
    var pageDiscount = ($('#discountAmount').val().replace(/,/g, "") - lineDisc).toFixed(digits);
    var pageSales = ($('#salesTaxAmount').val().replace(/,/g, "") - lineSales).toFixed(digits);
    var pageGrand = ($('#GrandTotal').val().replace(/,/g, "") - lineTotal).toFixed(digits);

    $('#Total').val(pageTotal);
    $('#discountAmount').val(pageDiscount);
    $('#salesTaxAmount').val(pageSales);
    $('#GrandTotal').val(pageGrand);

    $("#purchase-items_" + id).remove();
    //calculateTax(elementId);
    calculateexpense();
});



function RejectQty(counter) {
    debugger;
    var Rejval = $('#RejectedQty' + counter).val();
    var grnval = $('#GRNQty' + counter).val();
    var AccepQty = Number(Number(grnval) - Number(Rejval)).toFixed(4);
    $('#AcceptedQty' + counter).val(AccepQty);

    var rate = $('#Rate' + counter).val();
    var accQTY = $('#AcceptedQty' + counter).val();
    var value = Number(Number(rate.replace(/,/g, "")) * Number(accQTY.replace(/,/g, ""))).toFixed(4);
    $('#Total' + counter).val(value);
    var exchangeRate = $('#CurrencyExchangeRate').val();

    $('#FCValue' + counter).val(Number(Number(exchangeRate.replace(/,/g, "")) * Number(value.replace(/,/g, ""))).toFixed(4));
    var fc = $('#FCValue' + counter).val();
    var Expense = $('#Expense' + counter).val();
    $('#PKRValue' + counter).val(Number(Number(fc.replace(/,/g, "")) + Number(Expense.replace(/,/g, ""))).toFixed(4));

    var totalExpenses = 0.0000;
    var TotalQty = 0.0000;
    var division = 0.0000;

    $("input[name='AcceptedQty']").each(function () {
        TotalQty = (Number(TotalQty.replace(/,/g, "")) + Number($(this).val().replace(/,/g, ""))).toFixed(4);
       
    });

    var pkrValue = $('#PKRValue' + counter).val();

    $('#PKRRate' + counter).val((Number(pkrValue.replace(/,/g, "")) / Number(grnval.replace(/,/g, ""))).toFixed(4));
    totalExpenses = $("#exciseTaxAmount").val();
    division = Number(totalExpenses.replace(/,/g, "")) / Number(TotalQty.replace(/,/g, ""));
    $('#Expense' + counter).val((Number(division) * Number($('#AcceptedQty' + counter).val().replace(/,/g, ""))).toFixed(4));
    //calculateBalanceQty(counter);
    calculateGrandTotal();
    calculateexpense();



}

function calculateBalanceQty(counter) {
    debugger;
    var id = $('#Id').val();
    var irnId = $('#IRNItemId' + counter).val();
    var grnQty = $('#GRNQty' + counter).val().replace(/,/g, "");

    var grnval = $('#IRNQty' + counter).val().replace(/,/g, "");
    var rejQty = $('#RejectedQty' + counter).val().replace(/,/g, "");
    var accQty = $('#AcceptedQty' + counter).val().replace(/,/g, "");
    if (parseInt(grnItemQty) == 0 && id!=0) {
        grnItemQty = grnval;
    }
    $.ajax({
        type: 'GET',
        async: false,
        url: '/AP/Api/GetGRNBalanceQty?id=' + irnId
    }).done(function (data) {
        if (data != "NotFound") {
            debugger;
            $('#IRNQty' + counter).val(Number((data.received_Qty) - Number(data.grnQty) + Number(grnItemQty)).toFixed(4));
        }
    });

    var grnQty = $('#GRNQty' + counter).val().replace(/,/g, "");
    var grnval = $('#IRNQty' + counter).val().replace(/,/g, "");
    var rejQty = $('#RejectedQty' + counter).val().replace(/,/g, "");
    var accQty = $('#AcceptedQty' + counter).val();
    if (parseInt(grnQty) <= parseInt(grnval)) {
        if (parseInt(grnQty) != 0) {
            var AccepQty = Number(grnval) - Number(grnQty);
            $('#BalanceQty' + counter).val(Number(AccepQty).toFixed(4));
        } else {
            $('#BalanceQty' + counter).val((0).toFixed(4));
           // $('#AcceptedQty' + counter).val(Number(grnval).toFixed(4));
            $('#GRNQty' + counter).val(Number(grnval).toFixed(4));
            swal("", "GRN qty must be greater than 0", "warning");
        }
    } else {
        var acQty = grnQty - rejQty;
        $('#AcceptedQty' + counter).val(Number(grnval).toFixed(4));
        $('#GRNQty' + counter).val(Number(grnval).toFixed(4));
        $('#BalanceQty' + counter).val((0).toFixed(4));

        swal("", "GRN Qty Must be less than or Equal to IRN Qty", "warning");
    }

    var Rejval = $('#RejectedQty' + counter).val().replace(/,/g, "");
    var grnval = $('#GRNQty' + counter).val().replace(/,/g, "");
    var AccepQty = Number(Number(grnval) - Number(Rejval)).toFixed(4);
   // $('#AcceptedQty' + counter).val(AccepQty);

    var rate = $('#Rate' + counter).val().replace(/,/g, "");
    var accQTY = $('#AcceptedQty' + counter).val();
    // var value = Number(Number(rate) * Number(accQTY)).toFixed(4);
    var value = Number(Number(rate) * Number(grnval)).toFixed(4);
    $('#Total' + counter).val(value);
    var exchangeRate = $('#CurrencyExchangeRate').val().replace(/,/g, "");

    $('#FCValue' + counter).val(Number(Number(exchangeRate) * Number(value)).toFixed(4));
    var fc = $('#FCValue' + counter).val();
    var Expense = $('#Expense' + counter).val();
    $('#PKRValue' + counter).val(Number(Number(fc) + Number(Expense)).toFixed(4));

    var totalExpenses = 0.0000;
    var TotalQty = 0.0000;
    var division = 0.0000;

  //  $("input[name='AcceptedQty']").each(function () {
    $("input[name='GRNQty']").each(function () {
        TotalQty = (Number(TotalQty) + Number($(this).val().replace(/,/g, ""))).toFixed(4);

    });

    var pkrValue = $('#PKRValue' + counter).val();

    $('#PKRRate' + counter).val((Number(pkrValue) / Number(grnval)).toFixed(4));
    totalExpenses = $("#exciseTaxAmount").val();
    division = Number(totalExpenses) / Number(TotalQty);
 //   $('#Expense' + counter).val((Number(division) * Number($('#AcceptedQty' + counter).val())).toFixed(4));
    $('#Expense' + counter).val((Number(division) * Number($('#GRNQty' + counter).val().replace(/,/g, ""))).toFixed(4));
   // calculateLineTotal(counter);

    calculateGrandTotal()
}

function calculateTax(elementId) {
    var id = elementId.split('_');
    id = id[1];
    //tax slab defined
    var taxId = slabVal = $('#TaxSlab_' + id).val();
    //define tax values
    $.ajax({
        type: 'GET',
        async: false,
        url: '/AR/Api/GetTaxValues?id=' + taxId
    }).done(function (data) {
        if (data != "NotFound") {
            $('#SalesTaxPercentage_' + id).val((data.salesTaxPercentage).toFixed(4));
        }
        else {
            $('#SalesTaxPercentage_' + id).val(0.0000);
        }
        calculateLineTotal(id)
    });
}
function lineTotal() {
    var Rejval = $('#RejectedQty' + counter).val().replace(/,/g, "");
    var grnval = $('#GRNQty' + counter).val().replace(/,/g, "");
    var AccepQty = Number(Number(grnval) - Number(Rejval)).toFixed(4);
  //  $('#AcceptedQty' + counter).val(AccepQty);

    var rate = $('#Rate' + counter).val().replace(/,/g, "");
    var accQTY = $('#AcceptedQty' + counter).val().replace(/,/g, "");
   // var value = Number(Number(rate) * Number(accQTY)).toFixed(4);
    var value = Number(Number(rate) * Number(grnval)).toFixed(4);
    $('#Total' + counter).val(value);
    var exchangeRate = $('#CurrencyExchangeRate').val().replace(/,/g, "");

    $('#FCValue' + counter).val(Number(Number(exchangeRate) * Number(value)).toFixed(4));
    var fc = $('#FCValue' + counter).val().replace(/,/g, "");
    var Expense = $('#Expense' + counter).val().replace(/,/g, "");
    $('#PKRValue' + counter).val(Number(Number(fc) + Number(Expense)).toFixed(4));
    var totalExpenses = 0.0000;
    var TotalQty = 0.0000;
    var division = 0.0000;

    //$("input[name='AcceptedQty']").each(function () {
    $("input[name='GRNQty']").each(function () {
        TotalQty = (Number(TotalQty) + Number($(this).val().replace(/,/g, ""))).toFixed(4);
    });
    var pkrValue = $('#PKRValue' + counter).val();

    $('#PKRRate' + counter).val((Number(pkrValue) / Number(grnval)).toFixed(4));
    totalExpenses = $("#exciseTaxAmount").val().replace(/,/g, "");
    division = Number(totalExpenses) / Number(TotalQty);
  //  $('#Expense' + counter).val((Number(division) * Number($('#AcceptedQty' + counter).val())).toFixed(4));
    $('#Expense' + counter).val((Number(division) * Number($('#GRNQty' + counter).val().replace(/,/g, ""))).toFixed(4));
    calculateBalanceQty(counter);
    calculateGrandTotal();
    calculateexpense();
}
 
function calculateLineTotal(lineId) {
    var digits = 4; //up to two digit will be dynamic later.
    var taxPercentage = 0;
    var taxPercentage = 0;
    var lineTotal = 0;
    var total = $("#Total_" + lineId).val().replace(/,/g, "");
    total = (total == "" ? 0 : total);
    var discountPercentage = $('#DiscountPercentage_' + lineId).val();
    discountPercentage = (discountPercentage == "" ? 0 : discountPercentage);
    var discountAmount = ((discountPercentage / 100) * total).toFixed(digits);
    total = total - discountAmount;
    var taxPercentage = $('#SalesTaxPercentage_' + lineId).val();
    taxPercentage = (taxPercentage == "" ? 0 : taxPercentage);
    var taxAmount = ((taxPercentage / 100) * total).toFixed(digits);
    var lineTotal = (parseFloat(total) + parseFloat(taxAmount)).toFixed(digits);

    $("#DiscountAmount_" + lineId).val(discountAmount);
    $("#SalesTaxAmount_" + lineId).val(Number(taxAmount).toFixed(4));
     $("#LineTotal_" + lineId).val(Number(lineTotal).toFixed(4));
   RejectQty(counter);
    //calculateexpense();
    calculateBalanceQty(lineId);
    calculateGrandTotal();
}
function calculateGrandTotal() {
    //Calculating total of all the amounts at present page
    debugger;
    var totalAmount = 0;
    var Freight = 0;
    var totalExpense = 0;
    var salesTaxAmount = 0;
    Freight = $("#Freight").val().replace(/,/g, "");
    totalExpense = $("#exciseTaxAmount").val().replace(/,/g, "");
    
    var totalInFC = $("#Total1").val().replace(/,/g, "");
    var ExchangeRate = $("#CurrencyExchangeRate").val().replace(/,/g, "");
    $("#TotalValue").val(Number(Number(totalInFC) * Number(ExchangeRate)).toFixed(2));
    var totalValue = $("#TotalValue").val().replace(/,/g, "");
    //$("#TotalPKRValue").val(Number(Number(totalExpense) + Number(totalValue)).toFixed(4));
    $("#TotalPKRValue").val(Number(totalValue).toFixed(2));
    var tax = $("#totalSalesTax").val().replace(/,/g, "");
    var totalPKRValue = $("#TotalPKRValue").val();
    var sum = Number(tax) + Number(totalValue);
    //var sum = Number(Freight) + Number(tax) + Number(totalExpense) + Number(totalValue) + Number(tax);
    $("#GrandTotal").val(Number(sum).toFixed(2));
    $("#TotalPKRValue").val(Number(sum).toFixed(2));
}
