
function ValidateEmail(inputText) {
    //var mailformat = /^\w+([\.-]?\w+)*@@\w+([\.-]?\w+)*(\.\w{2,3})+$/;
    //if (inputText.match(mailformat)) {
    if (/^\w+([\.-]?\w+)*@@\w+([\.-]?\w+)*(\.\w{2,3})+$/.test(inputText)) {
        return true;
    }
    else {
        $('#msg1').html("Invalid Email address!");
        $('#Email').val('');
        $('#Email').focus();
        return false;
    }
}

function IsEmail(email) {
    var regex = /^([a-zA-Z0-9_\.\-\+])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    if (!regex.test(email)) {
        return false;
    } else {
        return true;
    }
}
function numberWithCommas(x) {
    return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}
function CheckEmail() {
    var emailstatus = false;
    //if ($('#Email').val() != "") {
    //    emailstatus = ValidateEmail($('#Email').val());
    //}
    //if (IsEmail($('#Email').val())==false)
    //{
    //    $('#msg1').html("Invalid Email address!");
    //    $('#Email').val('');
    //    $('#Email').focus();
    //    return false;
    //}    
    if ($('#Email').val().trim() != "") {
        $.ajax({
            type: "Get",
            url: "/EmployeeMaster/CheckUserEmailExist",
            datatype: "Json",
            data: { EmailId: $('#Email').val(),UserId:$('#EmployeeID').val()  },
            success: function (data) {
                debugger;
                console.log(data);
                if (data == "true") {
                    $("#msg1").html("Employee Emailid already exists!");
                    $("#msg1").show();
                    $('#Email').val('');
                    $('#Email').focus();
                    return false;
                }
                else {
                    $("#validations").hide();
                    return true;
                }

            }
        });
    }
}


function SaveClient() {
    debugger;
    $('#btnsave').attr('disabled', 'disabled');

    if ($('#ClientID').val() == 0) {
        
        //if (IsEmail($('#Email').val()) == false) {
        //    $('#msg1').html("Invalid Email address!");
        //    $('#Email').val('');
        //    $('#Email').focus();
        //    return false;
        //}
        //if (CheckEmail() == false)
        //    return false;
    }

    $('#msg1').hide();
    var RecPObj = {
        ClientID: $('#ClientID').val(),
        ClientName: $('#ClientName').val(),
        Address1: $('#Address1').val(),
        Address2: $('#Address2').val(),
        Address3: $('#Address3').val(),
        LocationName: $('#LocationName').val(),
        CountryName: $('#CountryName').val(),
        CountryID: $('#CountryID').val(),
        CityName: $('#CityName').val(),
        CityID: $('#CityID').val(),
        ContactName: $('#ContactName').val(),
        ContactNo: $('#ContactNo').val(),
        Email: $('#Email').val(),
        ClientType: $('#ClientType').val(),
        ClientPrefix: $('#ClientPrefix').val(),
        VATNo: $('#VATNo').val()

    }


    $.ajax({
        type: "POST",
        url: "/ClientMaster/SaveClient",
        datatype: "Json",
        data: { v: RecPObj },
        success: function (response) {
            if (response.status == "OK") {
                location.href = '/ClientMaster/Index';
                ////Swal.fire("Save Status", response.message, "success");
                //Swal.fire({ title: "Save Status", text: response.message }).then(
                //    function (t) {
                //        if (t.value) {
                //            location.href = '/EmployeeMaster/Create?id=' + response.EmployeeID;
                //        }
                //    });
            }
            else {
                $('#btnsave').removeAttr('disabled');
                Swal.fire("Save Status", response.message, "error");
                return false;
            }
        }
    });

}
(function ($) {

    'use strict';
    function initformControl() {
        $('#JoinDate').datepicker({
            dateFormat: 'dd-mm-yy',
            changeMonth: true,
            changeYear: true
        }).on('changeDate', function (e) {
            $(this).datepicker('hide');
        });



  

        $('#CountryName').autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: "/ZoneChart/GetCountryList",
                    data: { SearchText: request.term },
                    dataType: "json",
                    type: "GET",
                    success: function (data) {
                        response($.map(data, function (val, item) {
                            return {
                                label: val.CountryName,
                                value: val.CountryName
                            }
                        }))
                    }
                });
            },
            minLength: 1,
            autoFocus: false,
            select: function (event, ui) {
                event.preventDefault();
                $('#CountryName').val(ui.item.label);
                return false;
            },
            focus: function (event, ui) {
                $('#CountryName').val(ui.item.label);

                return false;
            }
        });

        setTimeout(function () {
            if ($('#ClientID').val() > 0) {
                $('#btnsave').val('Update');
             
                $('#Email').attr('readonly', 'readonly');
            }
            
        }, 100)

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
                            SaveClient();

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
      //  $("#EmployeeCode").attr('readonly', 'readonly');
        $("#ClientName").focus();

    })

})(jQuery)