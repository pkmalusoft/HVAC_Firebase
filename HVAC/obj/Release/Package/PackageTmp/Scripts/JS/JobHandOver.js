var _decimal = $('#hdncompanydecimal').val();
 



function SaveJobHandOver() {
    debugger;
    

    var po = {
        ID: $('#ID').val(),
        JobHandOverID: $('#JobHandOverID').val(),
        QuotationId: $('#QuotationId').val(),        
        PONumber: $('#PONumber').val(),
        PODate: $('#PODate').val(),
        VarNo: $('#VarNo').val(),
        OrderValue: $('#OrderValue').val(),
        VatPercent: $('#VatPercent').val(),
        VatAmount: $('#VatAmount').val(),
        TotalValue: $('#TotalValue').val()

    }
    
    $.ajax({
        type: "POST",
        url: '/InwardPO/SavePO/',
        datatype: "json",
        data: { obj:po },
        success: function (response) {
            if (response.status == "ok") {
                Swal.fire("Save Status!", response.message, "success");

                window.location.href = '/InwardPO/Index';


            }
            else {
                $('#btnSaveQuotation').removeAttr('disabled');
                Swal.fire("Save Status!", response.message, "warning");
                //window.location.reload();
            }

        }
    });
}
function showquotationdetails() {
    var quotationid = $('#QuotationID').val();
    if (quotationid == 0 || quotationid=='') {
        Swal.fire('Data Validation', 'Select Quotation No. to retrieve the Details!', 'info');
    }
    else {
        showQuotationEntry(quotationid);
    }    

}
function showcopyquotationdetails() {
    var quotationid = $('#QuotationID').val();
    if (quotationid == 0 || quotationid == '') {
        Swal.fire('Data Validation', 'Select Quotation No. to retrieve the Details!', 'info');
    }
    else {
        showQuotationEntry(quotationid);
    }
    

}
 
 
 
 
 
 
 
(function ($) {

    'use strict';
    function initformControl() {
        $('#PODate').datepicker({
            dateFormat: 'dd-mm-yy',
            changeMonth: true,
            changeYear: true
        }).on('changeDate', function (e) {
            $(this).datepicker('hide');
        });

        $('#CompletionDate').datepicker({
            dateFormat: 'dd-mm-yy',
            changeMonth: true,
            changeYear: true
        }).on('changeDate', function (e) {
            $(this).datepicker('hide');
        });
        
        $('#QuotationNo').autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: "/Quotation/GetQuotationNo",
                    data: { term: request.term, EnquiryID: $('#EnquiryID').val(), EmployeeID :$('#QEngineerID').val() },
                    dataType: "json",
                    type: "GET",
                    success: function (data) {
                        response($.map(data, function (val, item) {
                            return {
                                label: val.QuotationNo,
                                value: val.QuotationID,
                                Version:val.Version
                            }
                        }))
                    }
                });
            },
            minLength: 1,
            autoFocus: false,
            select: function (event, ui) {
                event.preventDefault();
                $('#QuotationID').val(ui.item.value);
                $('#Version').val(ui.item.Version);
                
                $('#QuotationNo').val(ui.item.label);
                
                return false;
            },
            focus: function (event, ui) {
                $('#QuotationID').val(ui.item.value);
                $('#Version').val(ui.item.Version);

                $('#QuotationNo').val(ui.item.label);

                return false;
            }
        });

        $('#EnquiryID').change(function () {
            if ($('#QuotationID').val() == 0) { //new mode

                //GetNewQuotationNo
                $.ajax({
                    url: "/Quotation/GetNewQuotationNo",
                    data: { EnquiryID: $('#EnquiryID').val(), EmployeeID: $('#QEngineerID').val() },
                    dataType: "json",
                    type: "GET",
                    success: function (response) {
                        console.log(response);
                        $('#QuotationNo').val(response.QuotationNo);
                        $('#QuotationNo').attr('readonly', 'readonly');
                        $('#Version').val(response.Version);
                        $('#Version').attr('readonly', 'readonly');
                        $('#ClientID').val(response.ClientID);
                        $('#ClientDetail').val(response.ClientDetail);
                        $('#QuotationNo').css('color', 'blue');
                    }
                });
            }

        })
       

        
    
   
 
     

    }
    function init() {
        initformControl();
    }
    $(document).ready(function () {
        init();

    })

})(jQuery);


(function () {
    "use strict";
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

                            SavePO();

                        }
                    },
                    !1
                );
            });
        },
        !1
    );
})(),
    $(document).ready(function () {
        $(".custom-validation").parsley();
      
    });
