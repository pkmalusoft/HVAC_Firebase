var _decimal = 2;
function setrowactiverev(index1) {
    var idtext = 'trrow_'
    $('[id^=' + idtext + ']').each(function (index, item) {
        $('#trrow_' + index).removeClass('rowactive');
    });
    $('#trrow_' + index1).addClass('rowactive');
}
function DeleteAllocation(id) {
    Swal.fire({ title: "Are you sure?", text: "Do you want to delete this Allocation?", icon: "warning", showCancelButton: !0, confirmButtonColor: "#34c38f", cancelButtonColor: "#f46a6a", confirmButtonText: "Yes, delete it!" }).then(
        function (t) {
            if (t.value) {

                $.ajax({
                    type: "POST",
                    url: '/ReceiptPaymentAllocation/DeleteAllocation/',
                    data: {
                        'ID': id
                    },
                    success: function (response) {
                        debugger;
                        if (response.Status == "OK") {
                            refreshallocation();
                        }
               }
                });
            }
        });
}
function numberWithCommas(nStr) {
    var _value = '';

    if (nStr != 'undefined' && nStr != null) {
        _value = nStr.toString().replaceAll(',', '');
    }
    _value = parseFloat(_value).toFixed(_decimal);
    //return _value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");

    _value += '';
    var x = _value.split('.');
    var x1 = x[0];
    var x2 = x.length > 1 ? '.' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + ',' + '$2');
    }
    return x1 + x2;
}

function parsenumeric(obj) {
    if ($(obj).val() == '') {
        return 0;
    }
    else {
        var _value = $(obj).val();

        if (_value != undefined) {
            _value = _value.replaceAll(',', '');
            return parseFloat(parseFloat(_value).toFixed(_decimal));
        }
        else {
            return 0;
        }
    }
}
function adjustunallocate(rowindex) {
    debugger;
    var itemcount = $('#PackItemBody > tr').length;
    var idtext = 'str_';
    if ($('#chkadjustallocate_' + rowindex).prop('checked') == false) {
        $('[id^=' + idtext + ']').each(function (index, item) {


            var childrowindex = $('#str_' + index).attr('parentrowindex');
            if (rowindex == childrowindex) {

                $('#hdnAdjustmentdeleted_' + index).val(true);
                $('#str_' + index).addClass('hide');
            }
            else {

            }

            if ((index + 1) == itemcount) {
                getTotalAdjustment(rowindex);
            }
        });
    } else {
        showadjustitempopup(rowindex);
    }

}
function showadjustitempopup(rowindex) {

    var title = 'Invoice NO.' + $('#hdnInvoiceNo_' + rowindex).val() + '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ' + $('#hdnInvoiceDate_' + rowindex).val();
    $('#h4PackListTitle').html(title);
    $('#ParentRowCount').val(rowindex);

    var itemcount = $('#PackItemBody > tr').length;
    var idtext = 'str_';

    if (itemcount > 0) {
        $('[id^=' + idtext + ']').each(function (index, item) {
            var deleted = $('#hdnAdjustmentdeleted_' + index).val();
            if (deleted != true && deleted != 'true') {
                var childrowindex = $('#str_' + index).attr('parentrowindex');
                if (rowindex == childrowindex) {
                    //var childrow = $('#str_' + index);
                    $('#str_' + index).removeClass('hide');
                }
                else {
                    $('#str_' + index).addClass('hide');
                }
            } else {
                $('#str_' + index).addClass('hide');
            }

            if ((index + 1) == itemcount) {
                $('#itempopup').modal('show');
                $('#OAdjustmentAcHeadID').val('').trigger('change');
                $("#OAdjustmentAmount").val('0');

                $('#OAdjustmentAcHeadID').focus();
                getTotalAdjustment(rowindex);
                setTimeout(function () {
                    var balance = parsenumeric($('#txtoutstanding_' + $('#ParentRowCount')));
                    var allocate = parsenumeric($('#txtamount_' + $('#ParentRowCount')))
                    var totaladjustment = parsenumeric($('#txtadjust_' + $('#ParentRowCount')));
                    if (totaladjustment == '') {
                        totaladjustment = 0;
                    }
                    var adjustmet = parseFloat(balance) - (parseFloat(allocate) + parseFloat(totaladjustment));
                    $('#OAdjustmentAmount').val(numberWithCommas(adjustmet));
                    $('#OAdjustmentAcHeadID').focus();
                }, 200)

            }
        });
    }
    else {
        $('#itempopup').modal('show');
        $('#OAdjustmentAcHeadID').val('').trigger('change');
        var balance = parsenumeric($('#txtoutstanding_' + $('#ParentRowCount').val()));
        var allocate = parsenumeric($('#txtamount_' + $('#ParentRowCount').val()));
        var totaladjustment = parsenumeric($('#txtadjust_' + $('#ParentRowCount').val()));
        if (totaladjustment == '') {
            totaladjustment = 0;
        }
        var adjustmet = parseFloat(balance) - (parseFloat(allocate) + parseFloat(totaladjustment));
        $('#OAdjustmentAmount').val(numberWithCommas(adjustmet));



        getTotalAdjustment(rowindex);
        setTimeout(function () {
            $('#OAdjustmentAcHeadID').focus();
        }, 200)

    }

}
function receiptallocation(index1,id) {
    setrowactiverev(index1);
    $.ajax({
        type: "POST",
        url: '/ReceiptPaymentAllocation/ShowReceiptAllocation/',
        datatype: "html",
        data: { id :id ,EntryType:$('#EntryType').val()},
        success: function (response) {
            debugger;
            $('#allocationcontainer').html(response);
            var autoallocate = true;
            var TotalAmount = 0;
            var recpayid = $("#RecPayID").val();
            if (autoallocate == true)
                TotalAmount = parseFloat($('#FMoney').val()).toFixed(3);
            if ($('#CustomerID').val() == 0) {
                alert('Select Customer!');
                return;
            }

            if ($('#FMoney').val() == "") {// || $('#FMoney').val()=="0") {
                //alert('Enter Receipt Amount!');
                $('#FMoney').val(0);
            }
            var ID = $('#CustomerID').val();
            $.ajax({
                type: "POST",
                url: '/ReceiptPaymentAllocation/GetReceiptAllocated/',
                data: {
                    'ID': ID, 'amountreceived': parseFloat(TotalAmount), 'RecPayId': recpayid, 'RecPayType': $('#RecPayType').val(),'EntryType':$('#rEntryType').val()
                },
                success: function (response) {
                    debugger;
                    var data = response.salesinvoice;
                    var advance = response.advance;
                    $('#hdnAdvance').val(advance);
                    $('#tbl1').html('');
                    if (data.length == 0) {
                        $('#BalanceAmount').val(parseFloat(advance).toFixed(2));
                    }
                    var totalallocated = 0;
                    var amt = 0;
                    for (var i = 0; i < data.length; i++) {
                        var date = new Date(data[i].date);
                        amt = parseFloat(amt) + parseFloat(data[i].Amount);
                        var tempdate = new Date(date).getDate() + '/' + (new Date(date).getMonth() + 1) + '/' + new Date(date).getFullYear();
                        var invoiceno = "'" + data[i].InvoiceNo + "'";
                        html = '<tr>' +
                            '<td><a href="JavaScript:void(0)" onclick="ShowInvoiceReceipts(' + invoiceno + ')" >' + data[i].InvoiceNo + '</a><input type="hidden" id="hdnInvoiceType_' + i + '"  value="' + data[i].InvoiceType + '" /><input id="hdnCreditNoteId"   value=' + data[i].CreditNoteID + ' type="hidden">  <input id="hdnAcOPInvoiceDetailID" value=' + data[i].AcOPInvoiceDetailID + ' type="hidden"><input id="hdnInvoiceId_' + i + '"    value=' + data[i].SalesInvoiceID + ' type="hidden"></td>' +
                            '<td>' + data[i].DateTime + '<input id="" name="CustomerRcieptChildVM[' + i + '].InvoiceDate" value=' + data[i].DateTime + ' type="hidden"></td>' +
                            '<td class="text-right">' + numberWithCommas(data[i].InvoiceAmount)  + '</td>' +
                            '<td class="text-right">' + numberWithCommas(data[i].AmountReceived)  + '</td>' +
                            '<td class="text-right">' + numberWithCommas(parseFloat(data[i].InvoiceAmount)-parseFloat(data[i].AmountReceived)) + '</td>' +
                            '<td class="text-right">' + numberWithCommas(data[i].Balance)  + '</td>'+
                            '<td class="text-right">' + numberWithCommas(data[i].AdjustmentAmount)  + '</td>' +
                            '<td class="text-right">' + '<a href="#" onclick="DeleteAllocation(' + data[i].RecPayDetailID + ')" class="text-danger"><i class="mdi mdi-delete font-size-18 text-danger"></i></a></td>';
                        html = html + '</tr>';
                        totalallocated = parseFloat(totalallocated) + parseFloat(data[i].Balance);
                        $('#spantotalallocate').html(numberWithCommas(totalallocated));
                        $('#tbl1').append(html);
                  

                    }

                }
            });
        }
        })
}
function ShowInvoiceReceipts(InvoiceNo) {
    var ID = $('#CustomerID').val();

    $.ajax({
        type: "POST",
        url: '/ReceiptPaymentAllocation/GetInvoiceReceipts/',
        data: {
            'CustomerID': ID, 'InvoiceNo' : InvoiceNo
        },
        success: function (response) {
            debugger;
            var data = response.salesinvoice;
            var advance = response.advance;
            
            $('#tbl2').html('');
            //if (data.length == 0) {
            //    Swal.fire('No ')
            //}

            var amt = 0;
            for (var i = 0; i < data.length; i++) {
                var date = new Date(data[i].RecPayDate);
              
                var tempdate = new Date(date).getDate() + '/' + (new Date(date).getMonth() + 1) + '/' + new Date(date).getFullYear();
              
                html = '<tr>' +
                    '<td>' + data[i].DocumentNo + '<input type="hidden" id="rhdnRecPayDetailID_' + i + '"  value="' + data[i].RecPayDetailID + '" /></td>' +
                    '<td>' + data[i].RecPayDate1 + '</td>' +
                    '<td class="text-right">' + parseFloat(data[i].ReceiptAmount).toFixed(_decimal) + '</td>' +
                    '<td class="text-right">' + parseFloat(data[i].Amount).toFixed(_decimal) + '</td>' +                    
                    '<td class="text-right">' + parseFloat(data[i].AdjustmentAmount).toFixed(_decimal) + '</td>' +
                    '<td class="text-right">' + '<a href="#" onclick="DeleteAllocation(' + data[i].RecPayDetailID + ')"  class="text-danger"><i class="mdi mdi-delete font-size-18 text-danger"></i></a></td>';

                html = html + '</tr>';

                $('#tbl2').append(html);


            }
            $('#popuptitle').html('Allocation of Invoice No. ' + InvoiceNo);
            $('#invoicepopup').modal('show');
        }
    });

}

function ShowSupplierInvoiceReceipts(InvoiceNo) {
    var ID = $('#SupplierID').val();

    $.ajax({
        type: "POST",
        url: '/ReceiptPaymentAllocation/GetSupplierInvoiceReceipts/',
        data: {
            'SupplierID': ID, 'InvoiceNo': InvoiceNo
        },
        success: function (response) {
            debugger;
            var data = response.salesinvoice;
            var advance = response.advance;

            $('#tbl2').html('');
            //if (data.length == 0) {
            //    Swal.fire('No ')
            //}

            var amt = 0;
            for (var i = 0; i < data.length; i++) {
                var date = new Date(data[i].RecPayDate);

                var tempdate = new Date(date).getDate() + '/' + (new Date(date).getMonth() + 1) + '/' + new Date(date).getFullYear();

                html = '<tr>' +
                    '<td>' + data[i].DocumentNo + '<input type="hidden" id="rhdnRecPayDetailID_' + i + '"  value="' + data[i].RecPayDetailID + '" /></td>' +
                    '<td>' + data[i].RecPayDate1 + '</td>' +
                    '<td class="text-right">' + parseFloat(data[i].ReceiptAmount).toFixed(_decimal) + '</td>' +
                    '<td class="text-right">' + parseFloat(data[i].Amount).toFixed(_decimal) + '</td>' +
                    '<td class="text-right">' + parseFloat(data[i].AdjustmentAmount).toFixed(_decimal) + '</td>' +
                    '<td class="text-right">' + '<a href="#" onclick="DeleteAllocation(' + data[i].RecPayDetailID + ')"  class="text-danger"><i class="mdi mdi-delete font-size-18 text-danger"></i></a></td>';

                html = html + '</tr>';

                $('#tbl2').append(html);


            }
            $('#popuptitle').html('Allocation of Invoice No. ' + InvoiceNo);
            $('#invoicepopup').modal('show');
        }
    });

}
function refreshallocation() {
    var recpayid = $("#RecPayID").val();
    var TotalAmount = parseFloat($('#FMoney').val()).toFixed(3);
    var ID = $('#CustomerID').val();
    $.ajax({
        type: "POST",
        url: '/ReceiptPaymentAllocation/GetReceiptAllocated/',
        data: {
            'ID': ID, 'amountreceived': parseFloat(TotalAmount), 'RecPayId': recpayid, 'RecPayType': $('#RecPayType').val(),EntryType:$('#rEntryType').val()
        },
        success: function (response) {
            debugger;
            var data = response.salesinvoice;
            var advance = response.advance;
            $('#hdnAdvance').val(advance);
            $('#tbl1').html('');
            if (data.length == 0) {
                $('#BalanceAmount').val(parseFloat(advance).toFixed(2));
            }

            var amt = 0;
            var totalallocated = 0;
            for (var i = 0; i < data.length; i++) {
                var date = new Date(data[i].date);
                amt = parseFloat(amt) + parseFloat(data[i].Amount);
                var tempdate = new Date(date).getDate() + '/' + (new Date(date).getMonth() + 1) + '/' + new Date(date).getFullYear();
                var invoiceno = "'" + data[i].InvoiceNo + "'";
                html = '<tr>' +
                    '<td><a href="JavaScript:void(0)" onclick="ShowInvoiceReceipts(' + invoiceno  + ')">' + data[i].InvoiceNo + '</a><input type="hidden" id="hdnInvoiceType_' + i + '"  value="' + data[i].InvoiceType + '" /><input id="hdnCreditNoteId"   value=' + data[i].CreditNoteID + ' type="hidden">  <input id="hdnAcOPInvoiceDetailID" value=' + data[i].AcOPInvoiceDetailID + ' type="hidden"><input id="hdnInvoiceId_' + i + '"    value=' + data[i].SalesInvoiceID + ' type="hidden"></td>' +
                    '<td>' + data[i].DateTime + '<input id="" name="CustomerRcieptChildVM[' + i + '].InvoiceDate" value=' + data[i].DateTime + ' type="hidden"></td>' +
                    '<td class="text-right">' + numberWithCommas(data[i].InvoiceAmount) + '</td>' +
                    '<td class="text-right">' + numberWithCommas(AmountReceived) + '</td>' +
                    '<td class="text-right">' + numberWithCommas((parseFloat(data[i].InvoiceAmount) - parseFloat(data[i].AmountReceived))) + '</td>' +
                    '<td class="text-right">' + numberWithCommas(data[i].Balance) + '</td>' +
                    '<td class="text-right">' + numberWithCommas(data[i].AdjustmentAmount) + '</td>' +
                    '<td class="text-right">' + '<a href="#" onclick="DeleteAllocation(' + data[i].RecPayDetailID + ')"  class="text-danger"><i class="mdi mdi-delete font-size-18 text-danger"></i></a></td>';

                html = html + '</tr>';
                totalallocated = parseFloat(totalallocated) + parseFloat(data[i].Balance);
                $('#spantotalallocate').html(parseFloat(totalallocated).toFixed(_decimal));
                $('#tbl1').append(html);


            }

        }
    });
}



function SaveAllocation() {
    debugger;
    var itemcount = $('#tbl1 > tr').length;
    var idtext = 'txtamount_';
    var Items = [];
    var Adjustmentitems = [];
    var adjustmentitemcount = $('#PackItemBody > tr').length;
    if (itemcount > 0) {
        $('[id^=' + idtext + ']').each(function (index, item) {
            var checked = $('#chkallocate_' + index).prop('checked');
            if (checked == true) {
                var _InvoiceID = $('#hdnInvoiceId_' + index).val();
                var _InvoiceNo = $('#hdnInvoiceNo_' + index).val();
                var _InvoiceDate = $('#hdnInvoiceDate_' + index).val();
                var _InvoiceType = $('#hdnInvoiceType_' + index).val();
                var _AcOPInvoiceDetailID = $('#hdnAcOPInvoiceDetailID_' + index).val();
                var _CreditNoteID = $('#hdnCreditNoteID_' + index).val();
                var _Amount = parsenumeric($('#txtamount_' + index));
                var _Adjustmentamount =parsenumeric($('#txtadjust_' + index));

                if (_Adjustmentamount == "") {
                    _Adjustmentamount = 0;
                }
                if (_Amount == "") {
                    _Amount = 0;
                }
                var data = {
                    InvoiceID: _InvoiceID,
                    InvoiceNo: _InvoiceNo,
                    InvoiceDate: _InvoiceDate,
                    InvoiceType : _InvoiceType,
                    AcOPInvoiceDetailID: _AcOPInvoiceDetailID,
                    CreditNoteID: _CreditNoteID,
                    Amount: _Amount,
                    AdjustmentAmount: _Adjustmentamount                    
                }

                Items.push(data);

            }
            if ((index + 1) == itemcount) {

                if (adjustmentitemcount > 0) {
                    for (i = 0; i < adjustmentitemcount; i++) {
                        var deleted = $('#hdnAdjustmentdeleted_' + i).val();
                        if (deleted != 'true' && deleted != true) {
                            var InvoiceType = $('#OhdnInvoiceType_' + i).val();
                            var OpInvoiceID = $('#OhdnAcOpInvoiceDetailD_' + i).val();
                            var CreditNoteID = $('#OhdnCreditNoteID_' + i).val();
                            var InvoiceID = $('#OhdnInvoiceID_' + i).val();
                            var Amount = parsenumeric($('#OAdjustmentAmount_' + i));
                            var AcHeadID = $('#OAdjustmentAcheadID_' + i).val();
                            var data = {
                                InvoiceType: InvoiceType,
                                InvoiceID: InvoiceID,
                                AcOpInvoiceDetailD: OpInvoiceID,
                                Amount: Amount,
                                AcHeadID: AcHeadID,
                                CreditNoteID: CreditNoteID
                            }
                            Adjustmentitems.push(data);
                        }
                    }
                }

                var ItemDetails = JSON.stringify(Items);
                $.ajax({
                    type: "POST",
                    url: '/ReceiptPaymentAllocation/SaveReceiptAllocation/',
                    datatype: "html",
                    data: { RecPayID: $('#RecPayID').val(), OtherReceipt: $('#OtherReceipt').val(), EntryType: $('#rEntryType').val(), Details: ItemDetails, AdjustmentDetails: JSON.stringify(Adjustmentitems) },
                    success: function (response) {
                        debugger;
                        if (response.Status == "OK") {
                            Swal.fire('Save Status', 'Data Saved Successfully',"success");
                            refreshallocation();
                        }
                    }
                });
            }
        });
    }
    else {
        return '';
    }
}


//Supplier Payments
function refreshPaymentallocation() {
 
    $.ajax({
        type: "POST",
        url: '/ReceiptPaymentAllocation/ShowPaymentAllocation/',
        datatype: "html",
        data: { id: $('#RecPayID').val(), EntryType: $('#EntryType').val() },
        success: function (response) {
            debugger;
            $('#allocationcontainer').html(response);
            var autoallocate = true;
            var TotalAmount = 0;
            var recpayid = $("#RecPayID").val();
            if (autoallocate == true)
                TotalAmount = parseFloat($('#FMoney').val()).toFixed(3);
            if ($('#SupplierID').val() == 0) {
                alert('Select Supplier!');
                return;
            }

            if ($('#FMoney').val() == "") {// || $('#FMoney').val()=="0") {
                //alert('Enter Receipt Amount!');
                $('#FMoney').val(0);
            }
            var ID = $('#SupplierID').val();
            $.ajax({
                type: "POST",
                url: '/ReceiptPaymentAllocation/GetSupplierAllocated/',
                data: {
                    'ID': ID, 'amountreceived': parseFloat(TotalAmount), 'RecPayId': recpayid, 'SupplierTypeId': $('#SupplierTypeId').val(), 'EntryType': $('#rEntryType').val()
                },
                success: function (response) {
                    debugger;
                    var data = response.salesinvoice;
                    var advance = response.advance;
                    $('#hdnAdvance').val(advance);
                    $('#tbl1').html('');
                    if (data.length == 0) {
                        $('#BalanceAmount').val(parseFloat(advance).toFixed(2));
                    }
                    var totalallocated = 0;
                    var amt = 0;
                    for (var i = 0; i < data.length; i++) {
                        var date = new Date(data[i].date);
                        amt = parseFloat(amt) + parseFloat(data[i].Amount);
                        var tempdate = new Date(date).getDate() + '/' + (new Date(date).getMonth() + 1) + '/' + new Date(date).getFullYear();
                        var invoiceno = "'" + data[i].InvoiceNo + "'";
                        html = '<tr>' +
                            '<td><a href="JavaScript:void(0)" onclick="ShowSupplierInvoiceReceipts(' + invoiceno + ')" >' + data[i].InvoiceNo + '</a><input type="hidden" id="hdnInvoiceType_' + i + '"  value="' + data[i].InvoiceType + '" /><input id="hdnCreditNoteId"   value=' + data[i].CreditNoteID + ' type="hidden">  <input id="hdnAcOPInvoiceDetailID" value=' + data[i].AcOPInvoiceDetailID + ' type="hidden"><input id="hdnInvoiceId_' + i + '"    value=' + data[i].SalesInvoiceID + ' type="hidden"></td>' +
                            '<td>' + data[i].DateTime + '<input id="" name="CustomerRcieptChildVM[' + i + '].InvoiceDate" value=' + data[i].DateTime + ' type="hidden"></td>' +
                            '<td class="text-right">' + parseFloat(data[i].InvoiceAmount).toFixed(_decimal) + '</td>' +
                            '<td class="text-right">' + parseFloat(data[i].AmountReceived).toFixed(_decimal) + '</td>' +
                            '<td class="text-right">' + (parseFloat(data[i].InvoiceAmount) - parseFloat(data[i].AmountReceived)).toFixed(_decimal) + '</td>' +
                            '<td class="text-right">' + parseFloat(data[i].Balance).toFixed(_decimal) + '</td>' +
                            '<td class="text-right">' + parseFloat(data[i].AdjustmentAmount).toFixed(_decimal) + '</td>' +
                            '<td class="text-right">' + '<a href="#" onclick="DeletePaymentAllocation(' + data[i].RecPayDetailID + ')" class="text-danger"><i class="mdi mdi-delete font-size-18 text-danger"></i></a></td>';
                        html = html + '</tr>';
                        totalallocated = parseFloat(totalallocated) + parseFloat(data[i].Balance);
                        $('#spantotalallocate').html(parseFloat(totalallocated).toFixed(_decimal));
                        $('#tbl1').append(html);


                    }

                }
            });
        }
    })
}
function DeletePaymentAllocation(id) {
    Swal.fire({ title: "Are you sure?", text: "Do you want to delete this Allocation?", icon: "warning", showCancelButton: !0, confirmButtonColor: "#34c38f", cancelButtonColor: "#f46a6a", confirmButtonText: "Yes, delete it!" }).then(
        function (t) {
            if (t.value) {

                $.ajax({
                    type: "POST",
                    url: '/ReceiptPaymentAllocation/DeleteAllocation/',
                    data: {
                        'ID': id
                    },
                    success: function (response) {
                        debugger;
                        if (response.Status == "OK") {
                            refreshPaymentallocation();
                        }
                    }
                });
            }
        });
}
function paymentallocation(index1, id) {
    setrowactiverev(index1);
    $.ajax({
        type: "POST",
        url: '/ReceiptPaymentAllocation/ShowPaymentAllocation/',
        datatype: "html",
        data: { id: id, EntryType: $('#EntryType').val() },
        success: function (response) {
            debugger;
            $('#allocationcontainer').html(response);
            var autoallocate = true;
            var TotalAmount = 0;
            var recpayid = $("#RecPayID").val();
            if (autoallocate == true)
                TotalAmount = parseFloat($('#FMoney').val()).toFixed(3);
            if ($('#SupplierID').val() == 0) {
                alert('Select Supplier!');
                return;
            }

            if ($('#FMoney').val() == "") {// || $('#FMoney').val()=="0") {
                //alert('Enter Receipt Amount!');
                $('#FMoney').val(0);
            }
            var ID = $('#SupplierID').val();
            $.ajax({
                type: "POST",
                url: '/ReceiptPaymentAllocation/GetSupplierAllocated/',
                data: {
                    'ID': ID, 'amountreceived': parseFloat(TotalAmount), 'RecPayId': recpayid, 'SupplierTypeId': $('#SupplierTypeId').val(), 'EntryType': $('#rEntryType').val()
                },
                success: function (response) {
                    debugger;
                    var data = response.salesinvoice;
                    var advance = response.advance;
                    $('#hdnAdvance').val(advance);
                    $('#tbl1').html('');
                    if (data.length == 0) {
                        $('#BalanceAmount').val(parseFloat(advance).toFixed(2));
                    }
                    var totalallocated = 0;
                    var amt = 0;
                    for (var i = 0; i < data.length; i++) {
                        var date = new Date(data[i].date);
                        amt = parseFloat(amt) + parseFloat(data[i].Amount);
                        var tempdate = new Date(date).getDate() + '/' + (new Date(date).getMonth() + 1) + '/' + new Date(date).getFullYear();
                        var invoiceno = "'" + data[i].InvoiceNo + "'";
                        html = '<tr>' +
                            '<td><a href="JavaScript:void(0)" onclick="ShowInvoiceReceipts(' + invoiceno + ')" >' + data[i].InvoiceNo + '</a><input type="hidden" id="hdnInvoiceType_' + i + '"  value="' + data[i].InvoiceType + '" /><input id="hdnCreditNoteId"   value=' + data[i].CreditNoteID + ' type="hidden">  <input id="hdnAcOPInvoiceDetailID" value=' + data[i].AcOPInvoiceDetailID + ' type="hidden"><input id="hdnInvoiceId_' + i + '"    value=' + data[i].SalesInvoiceID + ' type="hidden"></td>' +
                            '<td>' + data[i].DateTime + '<input id="" name="CustomerRcieptChildVM[' + i + '].InvoiceDate" value=' + data[i].DateTime + ' type="hidden"></td>' +
                            '<td class="text-right">' + parseFloat(data[i].InvoiceAmount).toFixed(_decimal) + '</td>' +
                            '<td class="text-right">' + parseFloat(data[i].AmountReceived).toFixed(_decimal) + '</td>' +
                            '<td class="text-right">' + (parseFloat(data[i].InvoiceAmount) - parseFloat(data[i].AmountReceived)).toFixed(_decimal) + '</td>' +
                            '<td class="text-right">' + parseFloat(data[i].Balance).toFixed(_decimal) + '</td>' +
                            '<td class="text-right">' + parseFloat(data[i].AdjustmentAmount).toFixed(_decimal) + '</td>' +
                            '<td class="text-right">' + '<a href="#" onclick="DeletePaymentAllocation(' + data[i].RecPayDetailID + ')" class="text-danger"><i class="mdi mdi-delete font-size-18 text-danger"></i></a></td>';
                        html = html + '</tr>';
                        totalallocated = parseFloat(totalallocated) + parseFloat(data[i].Balance);
                        $('#spantotalallocate').html(parseFloat(totalallocated).toFixed(_decimal));
                        $('#tbl1').append(html);


                    }

                }
            });
        }
    })
}

function SavePaymentAllocation() {
    debugger;
    var itemcount = $('#tbl1 > tr').length;
    var idtext = 'txtamount_';
    var Items = [];
    if (itemcount > 0) {
        $('[id^=' + idtext + ']').each(function (index, item) {
            var checked = $('#chkallocate_' + index).prop('checked');
            if (checked == true) {
                var _InvoiceID = $('#hdnInvoiceId_' + index).val();
                var _InvoiceNo = $('#hdnInvoiceNo_' + index).val();
                var _InvoiceDate = $('#hdnInvoiceDate_' + index).val();
                var _InvoiceType = $('#hdnInvoiceType_' + index).val();
                var _AcOPInvoiceDetailID = $('#hdnAcOPInvoiceDetailID_' + index).val();
                var _CreditNoteID = $('#hdnCreditNoteID_' + index).val();
                var _Amount = $('#txtamount_' + index).val();
                var _Adjustmentamount = $('#txtadjust_' + index).val();

                if (_Adjustmentamount == "") {
                    _Adjustmentamount = 0;
                }
                if (_Amount == "") {
                    _Amount = 0;
                }
                var data = {
                    InvoiceID: _InvoiceID,
                    InvoiceNo: _InvoiceNo,
                    InvoiceDate: _InvoiceDate,
                    InvoiceType: _InvoiceType,
                    AcOPInvoiceDetailID: _AcOPInvoiceDetailID,
                    CreditNoteID: _CreditNoteID,
                    Amount: _Amount,
                    AdjustmentAmount: _Adjustmentamount
                }

                Items.push(data);

            }
            if ((index + 1) == itemcount) {
                var ItemDetails = JSON.stringify(Items);
                $.ajax({
                    type: "POST",
                    url: '/ReceiptPaymentAllocation/SavePaymentAllocation/',
                    datatype: "html",
                    data: { RecPayID: $('#RecPayID').val(), EntryType: $('#rEntryType').val(), Details: ItemDetails },
                    success: function (response) {
                        debugger;
                        if (response.Status == "OK") {
                            Swal.fire('Save Status', 'Data Saved Successfully');
                            refreshPaymentallocation();
                        }
                    }
                });
            }
        });
    }
    else {
        return '';
    }
}
(function ($) {

    'use strict';
    function initformControl() {
       
 

        

    }
    function init() {
        initformControl();
    }
    window.addEventListener(
        "load",
        function () {
            var t = document.getElementsByClassName("needs-validation");
            Array.prototype.filter.call(t, function (e) {
                e.addEventListener(
                    "submit",
                    function (t) {

                        if (false === e.checkValidity()) {
                            e.classList.add("was-validated");
                        }
                        else {
                            t.preventDefault();
                            t.stopPropagation();
                            e.classList.remove("was-validated");
                         

                        }
                    },
                    !1
                );
            });
        },
        !1
    );
    $(document).ready(function () {
        init();
        $("#CustomerName").autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: '/CustomerMaster/GetCustomerName',
                    datatype: "json",
                    data: {
                        term: request.term
                    },
                    success: function (data) {
                        response($.map(data, function (val, item) {
                            return {
                                label: val.CustomerName,
                                value: val.CustomerName,
                                CustomerId: val.CustomerID,
                                type: val.CustomerType
                            }
                        }))
                    }
                })
            },
            minLength: 1,
            autoFocus: false,
            focus: function (event, ui) {
                $("#CustomerName").val(ui.item.value);

                $('#CustomerID').val(ui.item.CustomerId);
            },
            select: function (e, i) {
                e.preventDefault();
                $("#CustomerName").val(i.item.label);

                $('#CustomerID').val(i.item.CustomerId);
            },

        });


        
   
    })

})(jQuery)