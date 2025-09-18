var _decimal = $('#hdncompanydecimal').val();
function RestrictSpaceSpecial(e) {
    var k;
    document.all ? k = e.keyCode : k = e.which;
    //console.log(e.keyCode);
    if ((k >= 48 && k <= 57) || k == 46)
        return true;
    else
        return false;
    //return ((k > 64 && k < 91) || (k > 96 && k < 123) || k == 8 || k == 32 || (k >= 48 && k <= 57));
}


function PaymentModeChange() {
    debugger;
    var val = $("#PaymentMode").val();
    
    if (val == 1) { //Bank
      
        $("#ChequeNo").attr('required', 'required');
        $("#ChequeDate").attr('required', 'required');
        $("#ChequeNo").removeAttr('disabled');
        $("#ChequeDate").removeAttr('disabled');
       
        $('#RecPayAmount').removeAttr('readonly');
         
        $('#Amount').removeAttr('readonly');
        
        $('#AcOPInvoiceDetailID').val(0);
    } else if (val == 2) { //2 Cash
        
        $("#ChequeNo").val('');
        $("#ChequeDate").val('');
        $("#ChequeNo").removeAttr('required');
        $("#ChequeDate").removeAttr('required');
        $("#ChequeNo").attr('disabled', 'disabled');
        $("#ChequeDate").attr('disabled', 'disabled')
        
        $('#RecPayAmount').removeAttr('readonly');
       
            $('#Amount').removeAttr('readonly');
        
      
      

    }
    else if (val == 3) { //2 other expense
        
        $("#ChequeNo").val('');
        $("#ChequeDate").val('');
        $("#ChequeNo").removeAttr('required');
        $("#ChequeDate").removeAttr('required');
        $("#ChequeNo").attr('disabled', 'disabled');
        $("#ChequeDate").attr('disabled', 'disabled')
       
        $('#RecPayAmount').attr('disabled', 'disabled');
        
       
        
    }

}
function funExportToPDF(id) {

    $.ajax({
        url: '/CustomerReceipt/ReceiptReport',

        type: "GET",
        data: { id: id },
        dataType: "json",
        success: function (response) {
            if (response.result == "ok") {
                $('#frmPDF').attr('src', '/ReportsPDF/' + response.path); //''

    setTimeout(function () {        
        window.open('/ReportsPDF/' + response.path);        
    }, 1000);
} else {
    alert(resuponse.message);
    //hideLoading();
}
            },
        });

return false;
}
var savevalid = false;
function SaveSupplierPayment() {
    debugger;
    ValidateTotal();
    
    if ($('#PaymentMode').val() == 3)
        $('#StatusEntry').val('OT'); //for other expense
    else if ($('#PaymentMode').val() == 1) {
        $('#StatusEntry').val('BK');
    }
    else if ($('#PaymentMode').val() == 2) {
        $('#StatusEntry').val('CS');
    }
    if (savevalid == true) {
        $('#btnsave').attr('disabled', 'disabled');
        var RecPObj = {
            RecPayID: $('#RecPayID').val(),
            RecPayDate: $('#RecPayDate').val(),
            DocumentNo: $('#DocumentNo').val(),
            SupplierID: $('#SupplierID').val(),
            CashBank: $('#CashBank').val(),
            BankName: $('#BankName').val(),
            ChequeBank: $('#ChequeBank').val(),
            ChequeNo: $('#ChequeNo').val(),
            ChequeDate: $('#ChequeDate').val(),
            Remarks: $('#Remarks').val(),
            EXRate: parsenumeric($('#EXRate')),
            CurrencyId: $('#CurrencyId').val(),
            Amount: parsenumeric($('#Amount')),
            FMoney: parsenumeric($('#FMoney')), 
            AcOPInvoiceDetailID: $('#AcOPInvoiceDetailID').val(),
            PaymentMode: $('#PaymentMode').val(),
            OPRefNo: $('#OPRefNo').val(),
            StatusEntry :$('#StatusEntry').val()
        }

        var Items = [];
        var Adjustmentitems = [];
        var adjustmentitemcount = $('#PackItemBody > tr').length;
        var itemcount = $('#tbl1 > tr').length;
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
        if (itemcount > 0) {
            for (i = 0; i < itemcount; i++) {
                //var deleted = $('#hdndeleted_' + i).val();
                var adjust = parsenumeric($('#txtadjust_' + i));
                if (adjust == '')
                    adjust = 0;
                var amount = parsenumeric($("#txtinvoice_" + i))
                if (amount == '')
                    amount = 0;
                if (adjust > 0 || amount > 0) {
                    var data = {
                        RecPayDetailID: $('#hdnRecPayDetailID_' + i).val(),
                        InvNo: $("#hdnInvNo_" + i).val(),
                        InvoiceID: $("#hdnInvoiceId_" + i).val(),
                        AcOPInvoiceDetailID: $('#hdnAcOPInvoiceDetailID_' + i).val(),
                        strDate: $('#hdnInvoiceDate_' + i).val(),
                        InvoiceNo: $('#hdnInvoiceNo_' + i).val(),
                        LCBalance: parsenumeric($("#hdnLCBalance_" + i)),
                        FCBalance: parsenumeric($("#hdnFCBalance_" + i)),
                        CurrencyName: $("#hdnProvCurrency_" + i).val(),
                        EXRate: $('#hdnEXRate_' + i).val(),
                        EXRateDiff: $("#hdnEXRateDiff_" + i).val(),
                        FCAmount: parsenumeric($("#txtFCAmount_" + i)),
                        Amount: parsenumeric($("#txtinvoice_" + i)),
                        AdjustmentAmount: adjust,
                        InvoiceType :$('#hdnInvoiceType_' + i).val()
                    }

                    Items.push(data);
                }

                //var data= {Id: Id, SupplierID: SupplierID, InvoiceDate: InvoiceDate, InvoiceNo: $("#InvoiceNo").val(), Remarks: $('#Remarks').val(), Details: JSON.stringify(AWBItems) },

                if (i == (itemcount - 1)) {
                    $.ajax({
                        type: "POST",
                        url: "/SupplierPayment/SaveSupplierPayment",
                        datatype: "Json",
                        data: { RecP: RecPObj, Details: JSON.stringify(Items), AdjustmentDetails:JSON.stringify(Adjustmentitems)},
                        success: function (response) {
                            if (response.status == "OK") {

                                Swal.fire("Save Status", response.message, "success");
                                setTimeout(function () {
                                    if ($('#RecPayID').val() == 0) {
                                        location.href = '/SupplierPayment/Create?id=0';
                                    }
                                    else {
                                        location.href = '/SupplierPayment/Index';
                                    }
                                }, 200)

                            }
                            else {

                                Swal.fire("Save Status", response.message, "error");
                                return false;
                            }
                        }
                    });
                }
            }
        }
        else {
            $.ajax({
                type: "POST",
                url: "/SupplierPayment/SaveSupplierPayment",
                datatype: "Json",
                data: { RecP: RecPObj, Details: JSON.stringify(Items) },
                success: function (response) {
                    if (response.status == "OK") {

                        Swal.fire("Save Status", response.message, "success");
                        setTimeout(function () {
                            if ($('#RecPayID').val() == 0) {
                                location.href = '/SupplierPayment/Create?id=0';
                            }
                            else {
                                location.href = '/SupplierPayment/Index';
                            }
                        }, 200)

                    }
                    else {

                        Swal.fire("Save Status", response.message, "error");
                        return false;
                    }
                }
            });
        }
    }

}
function ValidateTotal() {
    debugger;
                var TotalAmount = parsenumeric($("#Amount")) + parsenumeric($('#hdnAdvance'));
                var idtext = 'txtinvoice_';
                var amt = 0;
                var adjustamt = 0;
                savevalid = true;

                if (TotalAmount == 0) {
                    savevalid = false;
                    $('#msg1').show();
                    $('#msg1').text('Received Amount Required!');
                    $('#btnsave').attr('disabled', 'disabled');
                    return true;
                }

                $('[id^=' + idtext + ']').each(function (index, item) {
                    //  var id = $(item).attr('id').split('_')[1];
                    if ($(item).val() == "" || $(item).val() == null) {
                        $(item).val(0);
                    }
                    var itemval = parsenumeric($(item));;
                    //itemval = itemval.replace(',', '');

                    var paidamount = parseFloat(itemval);
                    amt = parseFloat(amt) + parseFloat(paidamount);
                    var adjust = parsenumeric($('#txtadjust_' + index));
                    if (adjust == '')
                        adjust = 0;
                    adjustamt = parseFloat(adjustamt) + parseFloat(adjust);

                    if (parseFloat(amt) == 0) {
                        $('#btnsave').attr('disabled', 'disabled');
                    }

                    var alloamt = parseFloat(amt).toFixed(_decimal);
                    var balance1 = parseFloat(TotalAmount) - parseFloat(amt);
                    $('#BalanceAmount').val(numberWithCommas(balance1));
                    $('#AllocatedAmount').val(numberWithCommas(amt));
                    $('#AdjustmentAmount').val(numberWithCommas(adjustamt));

                    if (parseFloat(balance1) > 0) {
                        $('#spanAdvance').html(numberWithCommas(parseFloat(balance1)));
                    }
                    else {

                        $('#spanAdvance').html('');
                    }
                    
                    var payingamount = parsenumeric($('#Amount'));
                    var allocatedamount = parseFloat(amt); // .toFixed(_decimal);
                    var advance = 0;
                    if (parseFloat(parsenumeric($("#Amount"))) == 0) {                
                        savevalid = false;
                        $('#msg1').show();
                        $('#msg1').text('Received Amount Required!');
                        $('#btnsave').attr('disabled', 'disabled');
                    }
                    else if (allocatedamount > 0 && parseFloat(TotalAmount) < parseFloat(allocatedamount)) {
                        $('#btnsave').attr('disabled', 'disabled');
                        savevalid = false;
                        $('#msg1').show();
                        $('#msg1').text('Allocation amount should not be more than Received amount!');
                    }
                    else {
                        $('#btnsave').removeAttr('disabled');
                        savevalid = true;
                        $('#msg1').hide();
                        $('#msg1').text('');
                       }

                });


}
function allocate(obj) {
    debugger;
    var balamt = parsenumeric($('#BalanceAmount'));
    if (balamt == "")
        balamt = "0";

    var idindex = $(obj).attr('id').split('_')[1];
    var txinvoice = $('#txtinvoice_' + idindex);
    if (parseFloat(balamt) > 0) {

        var idtext = 'txtinvoice_' + idindex;

        var balance = parsenumeric($('#txtbalance_' + idindex));
        if (balance == "") {
            balance = 0;
        }
        var adjust = parsenumeric($('#txtadjust_' + idindex));
        if (adjust == "")
            adjust = 0;
        if (parseFloat(balance) > 0 && parseFloat(balamt) >= parseFloat(balance)) {
            balance = parseFloat(balance) - parseFloat(adjust);
        }
        else {
            balance = parseFloat(balamt); //parseFloat(balance) - parseFloat(adjust);
        }
        if ($(obj).is(':checked')) {
            $(txinvoice).val(numberWithCommas(balance));
        }
        else {
            $(txinvoice).val(parseFloat("0").toFixed(_decimal));
        }
        ValidateTotal();
    }
    else {
        $(txinvoice).val(parseFloat("0").toFixed(_decimal));
        ValidateTotal();
    }
}
function autoallocation() {
       
    var idtext = 'txtinvoice_';
    if ($("#AutoAllocate").is(':checked')) {
        var TotalAmount = parsenumeric($('#Amount'));
                 if (TotalAmount > 0) {
                        $('#btnsearch').trigger('click');
                        return;
                        var amt = 0;
                                $('[id^=' + idtext + ']').each(function (index, item) {
                                    if ($(item).val() == "" || $(item).val() == null) {
                                        $(item).val(0);
                                    }
                                    var itemval = parsenumeric($(item));//.val();
                                    itemval = itemval.replace(',', '');
                                    var idindex = $(item).attr('id').split('_')[1];
                                    var balance = parsenumeric($('#txtbalance_' + idindex))
                                    var balanceval = balance.replace(',', '');

                                    if (parseFloat(TotalAmount) > parseFloat(balanceval)) {
                                        $(item).val(numberWithCommas(balanceval));
                                TotalAmount = parseFloat(TotalAmount)- parseFloat(balanceval);
                                $('#chkallocate_' + idindex).prop('checked', true);
                            }
                            else if (parseFloat(TotalAmount) > 0) {
                                $(item).val(numberWithCommas(TotalAmount));
                                TotalAmount = 0;
                                $('#chkallocate_' + idindex).prop('checked', true);
                            }
                            else {
                                $(item).val(numberWithCommas(TotalAmount));
                                $('#chkallocate_' + idindex).prop('checked', false);
                                $('#chkallocate_' + idindex).attr('value', 'false');
                            }
                            ValidateTotal();
                        });

                }
                 else {
                        $('[id^=' + idtext + ']').each(function (index, item) {
                            if ($(item).val() == "" || $(item).val() == null) {
                                $(item).val(0);
                            }
                            var itemval = parsenumeric($(item));
                            //itemval = itemval.replace(',', '');
                            $(item).val("0.000");
                            //$('#chkallocate_' + idindex).prop('checked', false);
                            //$('#chkallocate_' + idindex).attr('value', 'false');
                            ValidateTotal();

                        });

                }
        }
        else {

                $('[id^=' + idtext + ']').each(function (index, item) {
                    if ($(item).val() == "" || $(item).val() == null) {
                        $(item).val(0);
                    }
                    var itemval = $(item).val();
                    //itemval = itemval.replace(',', '');
                    $(item).val("0.000");
                    $('#chkallocate_' + index).prop('checked', false);
                    $('#chkallocate_' + index).removeAttr('checked');
                    ValidateTotal();

                });


        }

}
function SetDecimal() {
    
    var idtext = 'txtinvoice_';

    $('[id^=' + idtext + ']').each(function (index, item) {

        if ($(item).val() == "" || $(item).val() == null) {
            $(item).val(0);
        }
        else {
            var amt = $(item).val();
            $(item).val(numberWithCommas(amt));
        }

    });

    idtext = 'txtadjust_';
    $('[id^=' + idtext + ']').each(function (index, item) {

        if ($(item).val() == "" || $(item).val() == null) {
            $(item).val(0);
        }
        else {
            var amt = $(item).val();
            $(item).val(numberWithCommas(amt));
        }
    });
    var totamt = $('#Amount').attr('ovalue');
    $('#Amount').val(numberWithCommas(totamt));
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
        showitempopup(rowindex);
    }

}
function showitempopup(rowindex) {

    var title = 'Invoice NO.' + $('#hdnInvoiceNo_' + rowindex).val() + '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ' + $('#hdnInvoiceDate_' + rowindex).val();
    $('#h4PackListTitle').html(title);
    $('#ParentRowCount').val(rowindex);
  
    var itemcount = $('#PackItemBody > tr').length;
    var idtext = 'str_';

    if (itemcount > 0) {
        $('[id^=' + idtext + ']').each(function (index, item) {
            var deleted = $('#hdnAdjustmentdeleted_' + index).val();
            if (deleted!=true && deleted != 'true') {
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
                    var balance = parsenumeric($('#txtbalance_' + $('#ParentRowCount').val()));
                    var allocate = parsenumeric($('#txtinvoice_' + $('#ParentRowCount').val()))
                    var totaladjustment = parsenumeric($('#txtadjust_' + $('#ParentRowCount').val()));
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
        var balance = parsenumeric($('#txtbalance_' + $('#ParentRowCount').val()));
        var allocate = parsenumeric($('#txtinvoice_' + $('#ParentRowCount').val()));
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
(function ($) {

    'use strict';
    function initformControl() {
        $('#RecPayDate').datepicker({
            dateFormat: 'dd-mm-yy',
            changeYear: false,            
            setStartDate:'01-01-2022',
            setEndDate:'11-12-2022'
        }).on('changeDate', function (e) {
            $(this).datepicker('hide');
        });
        $('#ChequeDate').datepicker({
            dateFormat: 'dd-mm-yy',
        }).on('changeDate', function (e) {
            $(this).datepicker('hide');
        });

        if ($('#RecPayID').val() > 0) {//edit mode
            //edit mode
            $('#btnsave').html('Update');
            $('#divothermenu').removeClass('hide');
            $('#divothermenu1').removeClass('hide');
            if ($("#StatusEntry").val() == "BK") {
               
            }
            else if ($("#StatusEntry").val() == "CS") {
                
               
            }
            else if ($("#StatusEntry").val() == "OT") {              
                    $('#PaymentMode').val(3).trigger('change');
                    
                    $('#ChequeNo').attr('disabled', 'disabled');
                    $('#ChequeDate').attr('disabled', 'disabled');
                    $('#OpRefNo').removeAttr('readonly');
             }
            PaymentModeChange();
            
            var exrate = parseFloat($("#EXRate").val());
            $("#ExRate").val(exrate.toFixed(_decimal));
            $('#SupplierTypeId').attr('disabled', 'disabled');
        }
        else {
            $('#divothermenu').addClass('hide');
            $('#divothermenu1').addClass('hide');
        }

    

        $("#BankName").autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: '/CustomerReceipt/GetHeadsForReceipt',
                    datatype: "json",
                    data: {
                        term: request.term ,PaymentMode: $('#PaymentMode').val()
                    },
                    success: function (data) {
                        response($.map(data, function (val, item) {
                            return {
                                label: val.AcHead,
                                value: val.AcHead,
                                AcHeadID: val.AcHeadID
                            }
                        }))
                    }
                })
            }, minLength: 1,
            select: function (e, i) {
                e.preventDefault();
                $("#BankName").val(i.item.label);
               
            }
        });
        $("#customerName").autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: '/SupplierPayment/GetSupplierName',
                    datatype: "json",
                    data: {
                        term: request.term, SupplierTypeId: $("#SupplierTypeId").val()
                    },
                    success: function (data) {
                        response($.map(data, function (val, item) {
                            return {
                                label: val.SupplierName,
                                value: val.SupplierName,
                                ID: val.SupplierID,
                            }
                        }))
                    }
                })
            },
            minLength: 1,
            autoFocus: true,
            select: function (e, i) {
                e.preventDefault();
                $("#customerName").val(i.item.label);
                $('#SupplierID').val(i.item.ID);
                $('#SupplierID').attr('label', i.item.label);
                //$('#hdnCustomerType').val(i.item.type);
                //if ($('#hdnCustomerType').val() == "CR") {
                //    $("#Consignor1").css('color', 'blue');
                //}
                //else if ($('#hdnCustomerType').val() == "CS") {
                //    $("#Consignor1").css('color', 'red');
                //}
                //else {
                //    $("#Consignor1").css('color', 'black');
                //}

            },

        });
        $('#btnsearch').click(function () {
            var autoallocate = $("#AutoAllocate").is(':checked');
            var TotalAmount = 0;
            var recpayid = $("#RecPayID").val();
            if (autoallocate == true)
                TotalAmount = parsenumeric($('#Amount'));
            if ($('#SupplierID').val() == 0) {
                alert('Select Vendor!');
                return;
            }
            if ($('#AdjustmentAmount').val() > 0) {
                $('#PackItemBody').html('');
                $('#msg1').show();
                $('#msg1').text('Reallocation cleared the Adjustments!');
            }
            if ($('#Amount').val() == "") {// || $('#RecPayAmount').val()=="0") {
                //alert('Enter Receipt Amount!');
                $('#Amount').val(0);
            }

            var ID = $('#SupplierID').val();
            $.ajax({
                type: "POST",
                url: '/SupplierPayment/GetTradeInvoiceOfSupplier/' + ID,
                data: {
                    'ID': ID, 'amountreceived': parseFloat(TotalAmount), 'RecPayId': recpayid, 'SupplierTypeId': $('#vendortype').val()
                },
                success: function (response) {

                    var data = response.salesinvoice;
                    var advance = response.advance;
                    $('#hdnAdvance').val(advance);
                    $('#tbl1').html('');
                    if (data.length == 0) {
                        $('#BalanceAmount').val(parseFloat(advance).toFixed(_decimal));
                       
                        Swal.fire("Search Status", "No Invoice found!", "error");
                        $('#tbl1').html('');
                        return false;
                    }
                    //$scope.Orders = data;
                    var amt = 0;
                    for (var i = 0; i < data.length; i++) {
                        var date = new Date(data[i].date);
                        amt = parseFloat(amt) + parseFloat(data[i].Amount);
                        var tempdate = (new Date(date).getMonth() + 1) + '/' + new Date(date).getDate() + '/' + new Date(date).getFullYear();
                        var invoiceno = "'" + data[i].InvoiceNo + "'";
                        var html = '<tr id="tr_' + i + '"  >' +
                            '<td>' + data[i].InvoiceNo + '<input id="hdnInvoiceType_' + i + '" type="hidden" name="CustomerReceiptChildVM[' + i + '].InvoiceType"  value="' + data[i].InvoiceType + '" />  <input id="hdnAcOPInvoiceDetailID_' + i + '"" name="CustomerReceiptChildVM[' + i + '].AcOPInvoiceDetailID" value=' + data[i].AcOPInvoiceDetailID + ' type="hidden"/><input id="hdnRecPayDetailID_' + i + '"" name="CustomerReceiptChildVM[' + i + '].RecPayDetailID" value=0 type="hidden"><input id="hdnInvoiceId_' + i + '"  name="CustomerReceiptChildVM[' + i + '].InvoiceID" value=' + data[i].SalesInvoiceID + ' type="hidden"></td>' +
                            '<td>' + data[i].DateTime + '<input invdate="' + data[i].strDate + '" id="hdnInvoiceDate_' + i + '" name="CustomerReceiptChildVM[' + i + '].InvoiceDate" value=' + data[i].DateTime + ' type="hidden"></td>' +
                            '<td class="text-right">' + numberWithCommas(parseFloat(data[i].InvoiceAmount).toFixed(_decimal)) + '<input id="" name="CustomerReceiptChildVM[' + i + '].AmountToBeRecieved" value=' + data[i].InvoiceAmount + ' type="hidden" class="AmountToBeRecieved"></td>' +
                            '<td class="text-right">' + numberWithCommas(parseFloat(data[i].AmountReceived).toFixed(_decimal)) + '<input id="" name="CustomerReceiptChildVM[' + i + '].Received" value=' + data[i].AmountReceived + ' type="hidden"></td>' +
                            '<td class="text-right">' + numberWithCommas(parseFloat(data[i].Balance).toFixed(_decimal)) + '<input id="txtbalance_' + i + '" name="CustomerReceiptChildVM[' + i + '].Balance" value=' + data[i].Balance + ' type="hidden"><input id="hdnInvoiceNo_' + i + '" name="CustomerReceiptChildVM[' + i + '].InvoiceNo" value=' + data[i].InvoiceNo + ' type="hidden"></td>';
                        //'<td>' + data[i].Amount + '<input id="" name="customerRcieptVM[' + i + '].Amount" value=' + data[i].Amount + ' type="hidden"></td>' +
                        if (data[i].Allocated == true) {
                            html = html + '<td><div class="input-group gap-3"><input type="checkbox" onchange="allocate(this);" checked id="chkallocate_' + i + '" /><input type="text" onchange="ValidateTotal()"  id="txtinvoice_' + i + '" class="amt form-control-sm textrightamount AmountReceived" name="CustomerReceiptChildVM[' + i + '].Amount"  value="' + numberWithCommas(parseFloat(data[i].Amount).toFixed(_decimal)) + '" /><div></td>';
                        }
                        else {
                            html = html + '<td><div class="input-group gap-3"><input type="checkbox" onchange="allocate(this);" id="chkallocate_' + i + '" /><input type="text" onchange="ValidateTotal()"  id="txtinvoice_' + i + '" class="amt form-control-sm textrightamount AmountReceived" name="CustomerReceiptChildVM[' + i + '].Amount"  value="' + numberWithCommas( parseFloat(data[i].Amount).toFixed(_decimal)) + '" /></div></td>';
                        }

                        

                        html = html + '<td><div class="d-flex gap-3"> ' +
                            '<input type="checkbox" onchange="adjustunallocate(' + i + ');"  id="chkadjustallocate_' + i + '" /><input type = "text" readonly onchange = "ValidateTotal()" id = "txtadjust_' + i + '" class="amt1 form-control-sm textrightamount AdjustmentAmount" name = "CustomerReceiptChildVM[' + i + '].AdjustmentAmount" />' +
                            '<a id="plusminus_@i" class=" waves-effect waves-light filter" style="cursor: pointer;" onclick="showitempopup(' + i + ')"><i class="fas fa-plus font-size-18"></i></a></div></td></tr>'
                        
                        $('#tbl1').append(html);
                        $("#txtinvoice_" +i).keypress(function (e) {
                            return RestrictSpaceSpecial(e);
                        })             
                        $("#txtadjust_" + i).keypress(function (e) {
                            return RestrictSpaceSpecial(e);
                        })        
                        $("#txtinvoice_" + i).change(function () {
                            debugger;
                            var x = $(this);
                            var str = parsenumeric($(this));//.val().trim();

                            str = str.toString().replaceAll(',', '');
                            var resultstr = '';
                            if (str != '' && str != null) {
                                //$(this).val($(this).val().trim());
                                //str = $(this).val();
                                //console.log(str);
                                var _decimal = $('#hdncompanydecimal').val();
                                var _numberformat = $('#hdnnumberformat').val();
                                $(this).attr('ovalue', str);

                                if (_numberformat == "Lakhs")
                                    resultstr = getnumberformatLakhs(parseFloat(str).toFixed(_decimal));
                                else
                                    resultstr = addCommas(parseFloat(str).toFixed(_decimal));
                                $(this).val(resultstr);
                                //$(this).val(parseFloat(str).toFixed(_decimal));
                            }
                            else {
                                $(this).val(parseFloat(0).toFixed(_decimal));
                            }


                        })        

                        $("#txtadjust_" + i).change(function () {
                            debugger;
                            var x = $(this);
                            var str = parsenumeric($(this));//.val().trim();

                            str = str.toString().replaceAll(',', '');
                            var resultstr = '';
                            if (str != '' && str != null) {
                                //$(this).val($(this).val().trim());
                                //str = $(this).val();
                                //console.log(str);
                                var _decimal = $('#hdncompanydecimal').val();
                                var _numberformat = $('#hdnnumberformat').val();
                                $(this).attr('ovalue', str);

                                if (_numberformat == "Lakhs")
                                    resultstr = getnumberformatLakhs(parseFloat(str).toFixed(_decimal));
                                else
                                    resultstr = addCommas(parseFloat(str).toFixed(_decimal));
                                $(this).val(resultstr);
                                //$(this).val(parseFloat(str).toFixed(_decimal));
                            }
                            else {
                                $(this).val(parseFloat(0).toFixed(_decimal));
                            }


                        })        
                        ValidateTotal();


                    }
                }
          

            });
        });

        $("#customerName").change(function () {
            if ($('#customerName').val() != $('#SupplierID').attr('label')) {
                $('#customerName').val('');
                $('#SupplierID').val(0);
                $('#customerName').focus();
            }

        });
    
        $('#PackClearItem').click(function () {

            $('#OAdjustmentAcHeadID').val('').trigger('change');
            $("#OAdjustmentAmount").val('0');

        })
        $('#FMoney').change(function () {
            debugger;
            var exrate = parsenumeric($('#EXRate'));
            var fmoney = parsenumeric($('#FMoney'));
            if (exrate == '')
                exrate = 0;
            if (fmoney == '')
                fmoney = 0;
            var lcamount = parseFloat(fmoney) * parseFloat(exrate);
            
            var oamount = parsenumeric($('#Amount'));
            if (parseFloat(oamount) == 0)
                $('#Amount').val(numberWithCommas(lcamount));
        })

        $('#EXRate').change(function () {
            debugger;
            var exrate = parsenumeric($('#EXRate'));
            var fmoney = parsenumeric($('#FMoney'));
            if (exrate == '')
                exrate = 0;
            if (fmoney == '')
                fmoney = 0;
            var lcamount = parseFloat(fmoney) * parseFloat(exrate);
            var oamount = parsenumeric($('#Amount'));
            if (parseFloat(oamount)==0)
               $('#Amount').val(numberWithCommas(lcamount));

        })
        $('#CurrencyId').change(function () {
            if ($('#CurrencyId').val() == $('#BranchCurrencyID').val()) {
                $('#EXRate').val(1);
                //$('#Amount').removeAttr('readonly');
                //$('#FMoney').attr('readonly','readonly');
            }
            else {
                //$('#FMoney').removeAttr('readonly');
                //$('#Amount').attr('readonly', 'readonly');

            }
                

        })
        $('#PackAddItem').click(function () {
            debugger;
            var Total = 0;
            var MainTotal = 0;

            var acheadid = $("#OAdjustmentAcHeadID").val();
            var Amount =parsenumeric($("#OAdjustmentAmount"));
            
            var exists = false;
            var acheadname = $("#OAdjustmentAcHeadID option:selected").text();
            if (Amount == '')
                Amount = 0;

            if (acheadid == "" || acheadid == 0) {
                $("#OAdjustmentAcHeadID").focus();
                Swal.fire('Data Validation', 'Please Select Adjustment Account and Amount!','error');
                return false;
            }

            if (Amount == "" || parseFloat(Amount) == 0) {
                $("#OAdjustmentAmount").focus();
                Swal.fire('Data Validation', 'Please Enter Adjustment Amount!','error');
                return false;
            }

            var balance = parsenumeric($('#txtbalance_' + $('#ParentRowCount').val()));
            var allocate = parsenumeric($('#txtinvoice_' + $('#ParentRowCount').val()));
            var totaladjustment =parsenumeric($('#txtadjust_' + $('#ParentRowCount').val()))
            if (totaladjustment == '')
            {
                totaladjustment = 0;
            }
            totaladjustment=parseFloat(totaladjustment)+ parseFloat(Amount);

            if (parseFloat(balance) < (parseFloat(allocate) + parseFloat(totaladjustment))) {
                Swal.fire('Data Validation', 'Total Adjustment and Allocation should not exceed the Outstanding!', 'error');
                var totaladjustment =parsenumeric($('#txtadjust_' + $('#ParentRowCount').val()));
                if (totaladjustment == '') {
                    totaladjustment = 0;
                }
                var adjustmet = parseFloat(balance) - (parseFloat(allocate) + parseFloat(totaladjustment));
                $('#OAdjustmentAmount').val(numberWithCommas(adjustmet));
                return false;
            }

            var parentrow = $('#ParentRowCount').val();
            var invoicetype = $('#hdnInvoiceType_' + parentrow).val();
            var invoiceID = $('#hdnInvoiceId_' + parentrow).val();
            var OPinvoiceID = $('#hdnAcOPInvoiceDetailID_' + parentrow).val();
            var RowCount = $('#PackItemBody > tr').length; // parseInt($('#ItemRowCount').val());
            var RowHtml = '<tr id="str_' + RowCount + '" parentrowindex="' + $('#ParentRowCount').val() + '"><td><input type="hidden"  value="' + invoicetype + '" id="OhdnInvoiceType_' + RowCount + '" /><input type="hidden"  value="' + OPinvoiceID + '" id="OhdnAcOpInvoiceDetailD_' + RowCount + '" /><input type="hidden"  value="' + invoiceID + '" id="OhdnInvoiceID_' + RowCount + '" /><input type="hidden" class="hdndeleted" value="false" id="hdnAdjustmentdeleted_' + RowCount + '" /><input type="hidden" class="ID"    id="OAdjustmentID_' + RowCount + '" value="0"/>';
            RowHtml = RowHtml + '<input type="hidden" class="form-control" parentrowindex="' + $('#ParentRowCount').val() + '" id="OAdjustmentAcheadID_' + RowCount + '"   value="' + acheadid + '"/><input type="hidden" class="form-control" parentrowindex="' + $('#ParentRowCount').val() + '" id="OAdjustmentAchead_' + RowCount + '"   value="' + acheadname + '"/>' + acheadname + '</td>';
            RowHtml = RowHtml + '<td style="text-align:right">' + '<input type="text" id="OAdjustmentAmount_' + RowCount + '"  class="form-control textrightamount" value="' + Amount + '" onchange="getTotalAdjustment(' + $('#ParentRowCount').val() + ')"/></td>';
            RowHtml = RowHtml + '<td style="text-align: center;"><a href="javascript:void(0)" class="text-danger" onclick="AdjusmentdeleteItemtrans(this)"  class="deleteallocrow" id="PackDeleteAllocationRow_' + RowCount + '"><i class="mdi mdi-delete font-size-18"></i></a></td>';
            RowHtml = RowHtml + '</tr>';

            $('#PackItemBody').append(RowHtml);
            
            $('#OAdjustmentAcHeadID').val('').trigger('change');
            $("#OAdjustmentAmount").val('0');
         
            $('#OAdjustmentAcHeadID').focus();
            getTotalAdjustment($('#ParentRowCount').val());
            var balance = parsenumeric($('#txtbalance_' + $('#ParentRowCount').val()));
            var allocate = parsenumeric($('#txtinvoice_' + $('#ParentRowCount').val()));
            var totaladjustment = parsenumeric($('#txtadjust_' + $('#ParentRowCount').val()));
            if (totaladjustment == '') {
                totaladjustment = 0;
            }
            var adjustmet = parseFloat(balance) - (parseFloat(allocate) + parseFloat(totaladjustment));
            $('#OAdjustmentAmount').val(numberWithCommas(adjustmet));
            ValidateTotal();
            $(".textrightamount").change(function () {
                 var x = $(this);
            var str = $(this).val().trim();
            str = str.replaceAll(',', '');
            var resultstr = '';
            if (str != '' && str != null) {
                //$(this).val($(this).val().trim());
                //str = $(this).val();
                //console.log(str);
                var _decimal = $('#hdncompanydecimal').val();
                var _numberformat = $('#hdnnumberformat').val();
                $(this).attr('ovalue', str);
                if (_numberformat=="Lakhs")
                    resultstr = getnumberformatLakhs(parseFloat(str).toFixed(_decimal));
                else
                    resultstr = addCommas(parseFloat(str).toFixed(_decimal));
                $(this).val(resultstr);
                //$(this).val(parseFloat(str).toFixed(_decimal));
            }
            else {
                $(this).val(parseFloat(0).toFixed(_decimal));
            }

            });
        })
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
                        debugger;
                        if (false === e.checkValidity()) {
                            e.classList.add("was-validated");
                        }
                        else {
                            t.preventDefault();
                            t.stopPropagation();
                            e.classList.remove("was-validated");
                            SaveSupplierPayment();
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

    })

})(jQuery)