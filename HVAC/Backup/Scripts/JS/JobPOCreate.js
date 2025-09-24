

var _decimal = $('#hdncompanydecimal').val();
function GenerateMRequest() {
    $('#mrpopup').modal('show');
}

function SaveMRequest() {
    //GenerateMRequest
    $('#btnAddRequest').attr('disabled', 'disabled');
    Swal.fire({ title: "Are you sure?", text: "This action will make automatic Material Request to Notify by the Store Incharge !", icon: "warning", showCancelButton: !0, confirmButtonColor: "#34c38f", cancelButtonColor: "#f46a6a", confirmButtonText: "Yes, Add Material Request" }).then(
        function (t) {
            if (t.value) {
                var _ClientPOID = $('#ID').val();
                var _Storekeeperid = $('#StoreKeeperID').val();
                $.ajax({
                    type: "Post",
                    url: '/InwardPO/GenerateMRequest/',
                    datatype: "json",
                    data: { ClientPOID: _ClientPOID, Storekeeperid: _Storekeeperid},
                    success: function (response) {
                        if (response.status == "OK") {
                            Swal.fire("Save Status!", response.message, "success");
                            $('#btnAddRequest').removeAttr('disabled');
                            $('#mrpopup').modal('hide');
                        }
                        else {
                            Swal.fire("Save Status!", response.message, "warning");
                            $('#btnAddRequest').removeAttr('disabled');
                        }
                    }
                });
            }
        });
}
function parseNumeric(value) {
    var num = parseFloat(value);
    return isNaN(num) ? 0 : num;
}
function calculateVatAndTotal() {
    let orderValue = parseNumeric($('#OrderValue').val());
    let vatPercent = parseNumeric($('#VatPercent').val());
    let vatAmount = parseNumeric($('#VatAmount').val());

    // If VatPercent is used, recalculate VatAmount
    if (document.activeElement.id === 'VatPercent') {
        vatAmount = (orderValue * vatPercent) / 100;
        $('#VatAmount').val(vatAmount.toFixed(2));
    }

    // If VatAmount is changed directly, just use it
    let totalValue = orderValue + vatAmount;
    $('#TotalValue').val(totalValue.toFixed(2));
}

function calculateBondValues() {
    let orderValue = parseNumeric($('#txtSalesValue').val());
    let vatPercent = parseNumeric($('#txtPercentage').val());
    let bondamount = 0;
    // If VatPercent is used, recalculate VatAmount
    if (document.activeElement.id === 'txtPercentage' || document.activeElement.id === 'txtSalesValue') {
        bondamount = Math.round((orderValue * vatPercent) / 100);
        $('#txtBondValue').val(bondamount.toFixed(3));
    }

    
}
function calculateExpiryDate(issueDateStr, dueDays) {
    // Split the string and convert to Date object
    const [day, month, year] = issueDateStr.split('-').map(Number);
    const issueDate = new Date(year, month - 1, day); // Month is 0-indexed

    // Add dueDays
    issueDate.setDate(issueDate.getDate() + parseInt(dueDays));

    // Format back to dd-MM-yyyy
    const dd = String(issueDate.getDate()).padStart(2, '0');
    const mm = String(issueDate.getMonth() + 1).padStart(2, '0');
    const yyyy = issueDate.getFullYear();

    return `${dd}-${mm}-${yyyy}`;
}

function DeleteBondEntry(index) {
    Swal.fire({ title: "Are you sure?", text: "You won't be able to revert this!", icon: "warning", showCancelButton: !0, confirmButtonColor: "#34c38f", cancelButtonColor: "#f46a6a", confirmButtonText: "Yes, delete it!" }).then(
    function (t) {
            if (t.value) {
                var _Bondid = $('#hdnBondID_' + index).val();
                $.ajax({
                    type: "POST",
                    url: '/InwardPO/DeleteBond/',
                    datatype: "html",
                    data: { id: _Bondid },
                    success: function (data) {
                        
                        $("#DetailContainer1").html(data);
                    }
                });
            }
});
}
function SaveBondItem() {
    debugger;

    if ($('#BondTypeId').val() == null || $('#BondTypeId').val()=='') {
        Swal.fire('Data Validation', 'Select Bond Type!', 'warning');
        return;
    }

    $('#btnaddbond').attr('disabled', 'disabled');
    var bonddetail = {
        ID: $('#BondID').val(),
        BondTypeID: $('#BondTypeId').val(),
        SalesValue: $('#txtSalesValue').val(),
        Percentage: $('#txtPercentage').val(),
        BondValue: $('#txtBondValue').val(),
        BondIssueDate: $('#txtIssueDate').val(),
        BondExpiryDate: $('#txtExpiryDate').val(),
        BondValidity: $('#txtDueDays').val(),
        JobHandOverID: $('#JobHandOverID').val(),
        JobPurchaseOrderDetailID:$('#ID').val()
    }
            
                $.ajax({
                    type: "POST",
                    url: '/InwardPO/SaveBond/',
                    datatype: "html",
                    data: { obj: bonddetail },
                    success: function (response) {
                        if (response.status == "ok") {
                            Swal.fire('Save Status', response.message, 'success');
                            $('#btnaddbond').removeAttr('disabled');                            
                            setTimeout(function () {
                                $.ajax({
                                    type: "POST",
                                    url: '/InwardPO/ShowBondList/',
                                    datatype: "html",
                                    data: { id: $('#ID').val() },
                                    success: function (data) {
                                        $("#DetailContainer1").html(data);
                                        $('#btnaddbond').removeAttr('disabled');
                                        
                                    }
                                });
                            }, 100)


                        }
                        else {
                            Swal.fire('Save Status', response.message, 'warning');
                            $('#btnaddbond').removeAttr('disabled');
                        }
                    }
                });
            
}

function SavePO() {
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

                window.location.href = '/InwardPO/Create?id=' + response.Id;


            }
            else {
                $('#btnSaveQuotation').removeAttr('disabled');
                Swal.fire("Save Status!", response.message, "warning");
                //window.location.reload();
            }

        }
    });
}
 
 
 
 
 
 
 
 
(function ($) {

    'use strict';
    function initformControl() {

        $('#txtIssueDate').datepicker({
            dateFormat: 'dd-mm-yy',
            //changeMonth: true,
            //changeYear: true,
            autoclose: true,
        }).on('changeDate', function (e) {
            $(this).datepicker('hide');
        });

        $('#PODate').datepicker({
            dateFormat: 'dd-mm-yy',
            changeMonth: true,
            changeYear: true
        }).on('changeDate', function (e) {
            $(this).datepicker('hide');
        });

        $("#txtIssueDate, #txtDueDays").on("change", function () {
            const issueDate = $("#txtIssueDate").val();
            var dueDays = $("#txtDueDays").val();
            if (dueDays == '')
                dueDays = 0;
            if (issueDate != null && dueDays > 0) {
                const expiryDate = calculateExpiryDate(issueDate, dueDays);
                $('#txtExpiryDate').val(expiryDate);
            }
        });

        $('#OrderValue, #VatPercent,#VatAmount').on('input', function () {
            calculateVatAndTotal();
        });

        $('#txtSalesValue, #txtPercentage').on('input', function () {
            calculateBondValues();
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
       

        $('#OrderValue, #VatPercent, #VatAmount').on('input', function () {
            calculateVatAndTotal();
        });
    
   
 
     

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
