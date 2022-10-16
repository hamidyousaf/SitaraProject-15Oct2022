//global array declaration
var ids = [];
$('.remove-item').click(function () {
    var element = this.id;
    var id = element.replace("remove-item", "");

    var itemId = $('#StoreIssueItemId' + id).val();
    if (itemId != null) {
        ids.push(itemId);
    }
    //for remove Id of Adjustmentitems
    $('#IdsDeleted').text(ids);
    $("#current-item" + id).remove();
});
//function getItemValues(itemId, val) {
//    var id = itemId.replace("ItemId", "");
//    var elementItem = $('#' + itemId).val();
//    $.ajax({
//        url: '/AR/Invoice/GetUOM?id=' + val,
//        type: 'GET'
//    }).done(function (data) {
//        $('#UOMId_' + id).val(data[0].id);
//        $('#UOM_Value' + id).val(data[0].uom);
//        $('#Rate_' + id).val(Math.round(data[0].avgRate));
//    });
//    //calling stock from this function
//    var wareHouseId = $('#WareHouseId').val();
//    var issueDate = $('#IssueDate').val();
//    var stock = getStock(elementItem, wareHouseId, issueDate);
//    stock.done(function (data) {
//        $('#Stock_' + id).val(data.toFixed(2));
//    });
//}
$('.calculate-total').on('keyup', function () {
    var split = (this.id).split('_');
    var id = split[1];
    var avgRate = parseFloat($('#Rate_' + id).val());
    var qty = parseFloat($('#Qty_' + id).val());
    $('#LineTotal_' + id).val((avgRate * qty).toFixed(2));
});


// qty < stock validation for store-issue-note.
function StoreIssueForm() {
    var i = true;
    $('input[name="Qty"]').each(function () {
        var split = (this.id).split('_');
        var id = split[1];
        var stock = parseFloat($('#Stock_' + id).val());
        var qty = parseFloat($('#Qty_' + id).val());
        if (qty > stock) {
            alert("'Qty' should be lower than 'stock'.");
            i = false;
            return false;
        }
    });
    if (i) $("#IssueFormId").submit();
}


// qty < stock validation store-issue-return.
function IssueReturnForm() {
    var i = true;
    $('input[name="Qty"]').each(function () {
        var split = (this.id).split('_');
        var id = split[1];
        var stock = parseFloat($('#Stock_' + id).val());
        var qty = parseFloat($('#Qty_' + id).val());
        if (qty > stock) {
            alert("'Qty' should be lower than 'stock'. hahaha");
            i = false;
            return false;
        }
    });
    if (i) $("#ReturnFormId").submit();
}

function isNumberKey(evt) {
    //evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode == 8 || charCode == 37) {
        return true;
    } else if (charCode == 46 && $(this).val().indexOf('+') != -1) {
        return false;
    } else if (charCode > 31 && charCode != 46 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    return true;
}