//global array declaration
var ids = [];
$('.remove-item').click(function () {
    var element = this.id;
    var id = element.replace("remove-item", "");

    var itemId = $('#AdjustmentItemId'+id).val();
    if (itemId != null) {
        ids.push(itemId);
       // console.log(ids);
    }
    //for remove Id of Adjustmentitems
    $('#IdsDeleted').text(ids);
    $("#current-item" + id).remove();
});
//function getItemValues(itemId, val) {
//    debugger
//    var id = itemId.replace("ItemId", "");
//    var elementItem = $('#' + itemId).val();
//    $.ajax({
//        url: '/AR/Invoice/GetUOM?id=' + val,
//        type: 'GET'
//    }).done(function (data) {
//        $('#UOMId_'+id).val(data[0].id);
//        $('#UOM_Value'+id).val(data[0].uom);
//        $('#Rate_' + id).val(data[0].rate);
       
//    });
//    //calling stock from this function
//    var wareHouseId = $('#WareHouseId').val();
//    var adjustmentDate = $('#AdjustmentDate').val();
//    // debugger;
//    var stock = getStock(elementItem, wareHouseId, adjustmentDate);
//    stock.done(function (data) {
//        $('#Stock_' + id).val(data.toFixed(2));
//    });
//}
$('.calculate-total').on('keyup', function () {
    debugger
    var split = (this.id).split('_');
    var id = split[1];
    var stock = parseFloat($('#UOM_Stock').val());
    var qty = parseFloat($('#PhysicalStock_' + id).val());
    //var physicalStock = $(this).val();
   // var stockPlusPhysicalStock = stock + physicalStock;  
    $('#Balc').val((stock + qty).toFixed(2));
    var rate = parseFloat($('#Rate_').val());
    $('#LineTotal_' + id).val((rate * qty).toFixed(2));
});



