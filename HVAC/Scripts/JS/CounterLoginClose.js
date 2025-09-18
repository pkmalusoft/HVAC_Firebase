var _decimal = "2";
 
function SaveCounterLogin() {
    debugger;
    if ($('#CounterNo').val() == '' || $('#ShiftNo').val() == '') {

        
        $('#spanloginerror').html('Enter Counter No.and Shift No.to save');
        $('#CounterNo').focus();
        return false;

    }
    var model = {
        CounterLoginID: $('#CounterLoginID').val(),
        CounterID: $('#CounterID').val(),
        ShiftID: $('#ShiftID').val(),
        EmployeeID: $('#EmployeeID').val(),
        SalesDate: $('#SalesDate').val(),
        OpeningBalance: $('#OpeningBalance').val(),
        ClosingBalance: $('#ClosingBalance').val(),
        ClosedBy: $('#ClosedBy').val(),
        AccountPassword: $('#txtAccountPwd').val()
        
    }

    $('#spanloginerror').html('');
    $.ajax({
        type: "POST",
        url: "/ShiftEntry/SaveCounterClose/",
        datatype: "Json",
        data: {vm:model },
        success: function (response) {
            if (response.status == "OK") {
                //Swal.fire("Save Status", response.message, "success");
               
                window.location.href = "/ShiftEntry/Index";
                
            }
            else {
                //$('#btnsave').removeAttr('disabled');
                Swal.fire("Save Status", response.message, "error");
                return false;
            }
        }
    });
}

function GetCounterLogin() {
    debugger;
    $.ajax({
        type: "POST",
        url: "/SalesEntry/GetCounterLoginDetail/",
        datatype: "Json",
    
        success: function (response) {
            if (response.status == "OK") {
                //Swal.fire("Save Status", response.LoginDetail, "success");
                $('#EmployeeName').val(response.PersonName);
                $('#CounterID').val(response.CounterID);
                $('#ShiftID').val(response.ShiftID);
                $('#SalesPersonID').val(response.SalesPersonID);
                GetCounterAccount();
                $('#h4LoginDetail').html('Counter No. ' + response.CounterNo + ' Shift No.' + response.ShiftNo);
            }
            else {
                $('#CounterID').val(0);
                $('#SalesPersonID').val(0);
                $('#ShiftID').val(0);
                $('#EmployeeName').val(response.PersonName);
                $('#h4LoginDetail').html('Add Counter Login Detail');
                $('#btnlogindetail').trigger('click');
                $('#btnsave').attr('disabled', 'disabled');
                return false;
            }
        }
    });
}
function GetCounterAccount() {
    if ($('#PaymentModeID').val() == '')
        $('#PaymentModeID').val(1).trigger('change');
    debugger;
    $.ajax({
        type: "POST",
        url: "/SalesEntry/GetCounterAcHead/",
        datatype: "Json",
        data: { CounterID: $('#CounterID').val(), PaymentModeID: $('#PaymentModeId').val() },
        success: function (response) {
            if (response.status == "OK") {
                //Swal.fire("Save Status", response.LoginDetail, "success");
                $('#AcHeadId').val(response.AcHeadID);
                $('#AcHead').val(response.AcHeadName);
                $('#btnsave').removeAttr('disabled');
                $('#CustomerName').focus();
                
            }
            else {
                
                $('#AcHeadId').val(0);
                $('#EmployeeName').val('No Account Mapping');
                $('#btnsave').attr('disabled', 'disabled');

                return false;
            }
        }
    });
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

 
 
(function ($) {

    'use strict';
    function initformControl() {
          
        if ($('#CounterLoginID').val() > 0) {
            $('#btnsave').val('Close Sales');            
            $('#ClosedBy').focus();
        }
        else {
            
            $('#CounterID').focus();
        }
       

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
                            SaveCounterLogin();

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