function SaveProduct() {
    
    $('#btnsave').attr('disabled', 'disabled');
    var accountobj = {
        ProductID: $('#ProductID').val(),
        ProductCategoryId: $('#ProductCategoryId').val(),
        ProductGroupID: $('#ProductGroupID').val(),
            ProductName: $('#ProductName').val(),
            UnitID: $('#UnitID').val(),
            SalesRate: $('#SalesRate').attr('ovalue'),
            CostRate: $('#CostRate').attr('ovalue'),
            SalesAcHeadID: $('#SalesAcHeadID').val(),
            CostAcHeadID: $('#CostAcHeadID').val(),
            TaxPercent: $('#TaxPercent').val(),
            SGST: parsenumeric($('#SGST')),
            CGST: parsenumeric($('#CGST')),
            IGST: parsenumeric($('#IGST')),
            BranchID: $('#BranchID').val(),
            Barcode : $('#Barcode').val()

        }
    $.ajax({
        type: "POST",
        url: '/Product/SaveProduct/',
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
                    if ($('#ProductID').val() == 0) {
                        window.location.href = '/Product/Create?id=0';
                    }
                    else {
                        window.location.href = '/Product/Create?id=' + response.ProductID;
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

     

       

       
       
        


        


       
 

 

     

        if ($("#ProductTypeID").val() > 0) {
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

                            SaveProduct();



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
