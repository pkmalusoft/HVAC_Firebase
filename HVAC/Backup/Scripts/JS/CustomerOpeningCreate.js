var _decimal = "2";
function parsenumeric(obj) {
    if ($(obj).val() == '') {
        return 0;
    }
    else {
        var _value = $(obj).val();
        _value = _value.replaceAll(',', '');
        return parseFloat(parseFloat(_value).toFixed(_decimal));
    }
}
//millions format
function addCommas1(nStr) {

    nStr += '';
    var x = nStr.split('.');
    var x1 = x[0];
    var x2 = x.length > 1 ? '.' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + ',' + '$2');
    }
    return x1 + x2;
}

function getnumberformatLakhs1(nStr) {
    var x = nStr.split('.');
    var x1 = x[0];
    var x2 = x.length > 1 ? '.' + x[1] : '';
    var lastThree = x1.substring(x1.length - 3);
    var otherNumbers = x1.substring(0, x1.length - 3);
    if (otherNumbers != '')
        lastThree = ',' + lastThree;
    var res = otherNumbers.replace(/\B(?=(\d{2})+(?!\d))/g, ",") + lastThree + x2;
    return res;
}
function numberWithCommas(obj) {
    //nStr += '';
    //var x = nStr.split('.');
    //var x1 = x[0];
    //var x2 = x.length > 1 ? '.' + x[1] : '';
    //var rgx = /(\d+)(\d{3})/;
    //while (rgx.test(x1)) {
    //    x1 = x1.replace(rgx, '$1' + ',' + '$2');
    //}
    //return x1 + x2;


    
    var x = $(obj);
    var str = $(obj).val().trim();
    str = str.replaceAll(',', '');
    var resultstr = '';
    if (str != '' && str != null) {
        
        var _decimal = $('#hdncompanydecimal').val();
        var _numberformat = $('#hdnnumberformat').val();
        $(obj).attr('ovalue', str);
        if (_numberformat == "Lakhs")
            resultstr = getnumberformatLakhs1(parseFloat(str).toFixed(_decimal));
        else
            resultstr = addCommas1(parseFloat(str).toFixed(_decimal));
        $(obj).val(resultstr);
        //$(this).val(parseFloat(str).toFixed(_decimal));
    }
    else {
        $(obj).val(parseFloat(0).toFixed(_decimal));
    }

}
function SaveOpening() {
    debugger;
    if (parseFloat($('#txtDebit').val()) == 0 && parseFloat($('#txtCredit').val())==0)
    {
        Swal.fire("Save Status!", "Enter Amount", "error");
        return false;
    }
    $('#btnsave').attr('disabled', 'disabled');
    var dataobj= {
        AcOPInvoiceMasterID: $('#AcOPInvoiceMasterID').val(),
        AcOPInvoiceDetailId: $('#AcOPInvoiceDetailId').val(),
        InvoiceType: $('#StatusSC').val(),
        PartyID: $('#PartyID').val(),
        Debit: parsenumeric($('#txtDebit')),
        Credit: parsenumeric($('#txtCredit')),
        InvoiceNo: $('#InvoiceNo').val(),
        InvoiceDate: $('#InvoiceDate1').val(),
        AcHeadID: $('#AcHeadID').val(),
        Remarks: $('#Remarks').val()
    }
    $.ajax({
        type: "POST",
        url: "/CustomerOpening/SaveOpeningInvoice",
        datatype: "Json",
        data: { model: dataobj },        
        success: function (response) {
            if (response.status == "ok") {
                
                Swal.fire("Save Status!", response.message, "success");
                setTimeout(function () {
                    window.location.reload();
                }, 300)
            } else {
                Swal.fire("Save Status!", response.message, "error");
                $('#btnsave').removeAttr('disabled');
            }
        }
    });
}
(function ($) {

    'use strict';
    function initformControl() {
        $('#OpeningDate').datepicker({
            dateFormat: 'dd-mm-yy',
        }).on('changeDate', function (e) {
            $(this).datepicker('hide');
        });
        $('#InvoiceDate1').datepicker({
            dateFormat: 'dd-mm-yy',
        }).on('changeDate', function (e) {
            $(this).datepicker('hide');
        });
        

        $("#PartyName").autocomplete({
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
                $("#PartyName").val(ui.item.value);
                $('#PartyID').val(ui.item.CustomerId);
            },
            select: function (e, i) {
                e.preventDefault();
                $("#PartyName").val(i.item.label);
                $('#PartyID').val(i.item.CustomerId);
            },

        });

        $("#PartyName").focus();
        //$('#txtDebit').val(numberWithCommas($('#txtDebit').val()));
        //$('#txtCredit').val(numberWithCommas($('#txtCredit').val()));
        numberWithCommas($('#txtDebit'));
        numberWithCommas($('#txtCredit'));
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
                            SaveOpening();

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