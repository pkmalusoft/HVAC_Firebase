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
function SaveProductOpening() {
    
    $('#btnsave').attr('disabled', 'disabled');
    var accountobj = {
        OpeningID: $('#OpeningID').val(),
        EquipmentTypeID: $('#eEquipmentType').val(),
        Model: $('#Model').val(),
            ItemUnitID: 1,
            Quantity: $('#Quantity').val(),
            Rate: parsenumeric($('#Rate')),
        Value: $('#Value').val(),
            AsonDate :$('#AsonDate').val()
            

        }
    $.ajax({
        type: "POST",
        url: '/ProductOpening/SaveProductOpening/',
        datatype: "json",
        data: { v: accountobj },
        success: function (response) {
            debugger;
            if (response.status == "OK") {
                Swal.fire("Save Status!", response.message, "success");
                //$('#divothermenu').removeClass('hide');
                $('#btnSaveAccounts').removeAttr('disabled');
                var t = document.getElementsByClassName("needs-validation");
                $(t).removeClass('was-validated');
                setTimeout(function () {
                    if ($('#OpeningID').val() == 0) {
                        window.location.href = '/ProductOpening/Create?id=0';
                    }
                    else {
                        window.location.href = '/ProductOpening/Index';
                    }

                },100)
                
            }
            else {
                $('#btnsave').removeAttr('disabled');
                Swal.fire("Save Status!", response.message, "warning");
                //window.location.reload();
            }


        }
    });
}
 

 
 


(function ($) {

    'use strict';
    function initformControl() {
        var _decimal = "2";
        var accounturl = '/Accounts/GetHeadsForCash';
        //$('#transdate').datepicker({
        //    dateFormat: 'dd-mm-yy'
        //});

     

       

       
       
        


        


       
 

 

     

        if ($("#OpeningID").val() > 0) {
            $('#btnsave').html('Update');
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
                        debugger;
                        if (false === e.checkValidity()) {
                            e.classList.add("was-validated");
                        }
                        else {
                            
                            t.preventDefault();
                            t.stopPropagation();
                            e.classList.remove("was-validated");

                            SaveProductOpening();



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
