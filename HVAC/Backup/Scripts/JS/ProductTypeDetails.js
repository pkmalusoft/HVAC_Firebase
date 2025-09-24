 

function AddScopePopup(ID) {
    $.ajax({
        type: "POST",
        url: '/ProductType/ShowAddScopeofWork/',
        datatype: "html",
        data: { ID: ID, EquipmentTypeID: $('#EquipmentTypeID').val() },
        success: function (data) {
            $("#scopeContainer").html(data);
            
            $('#scopepopup').modal('show');
            $('#txtModel1').focus();
        }
    });
}

function AddWarranty(ID) {
    $.ajax({
        type: "POST",
        url: '/ProductType/ShowAddWarranty/',
        datatype: "html",
        data: { ID: ID, EquipmentTypeID: $('#EquipmentTypeID').val()  },
        success: function (data) {
            $("#warrantyContainer").html(data);
            $('#warrantypopup').modal('show');
        }
    });
}

function AddExclusion(ID) {
    $.ajax({
        type: "POST",
        url: '/ProductType/ShowAddExclusion/',
        datatype: "html",
        data: { ID: ID, EquipmentTypeID: $('#EquipmentTypeID').val()  },
        success: function (data) {
            $("#exclusionContainer").html(data);
            $('#exclusionpopup').modal('show');
        }
    });
}
/*New Popup Show Items Working */


function SaveSCopePopup() {
    debugger;
    var scopeoworkid = $('#scopeofworkid').val();
    var content = tinymce.get('txtDescription1').getContent()

    var obj = {
        ID: scopeoworkid,        
        Model: $('#txtModel1').val(),
        EquipmentTypeID: $('#EquipmentTypeID').val(),
        Description: content,
        OrderNo: $('#txtOrderNo1').val(),
        //Airmech: $('#ChkResAirmech').prop('checked'),
        //client: $('#ChkResClient').prop('checked')
    }

    // NOT stringified again
    // You can add AJAX here to save the changes to the server
    $.ajax({
        type: "POST",
        url: "/ProductType/SaveScopeItem1",
        contentType: "application/json;",
        dataType: "json",
        data: JSON.stringify(obj),

        //headers: { 'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() },
        success: function (data) {
            debugger;
            console.log(data);
            $('#scopepopup').modal('hide');
            $.ajax({
                type: "POST",
                url: '/ProductType/ShowScopeofWork/',
                datatype: "html",
                data: { EquipmentTypeID: $('#EquipmentTypeID').val() },
                success: function (data) {
                    $("#DetailContainer1").html(data);

                    $('#txtDescription1').val('');
                    $('#txtDescription1').focus();
                }
            });

        }
    });

}

function SaveWarrantyPopup() {
    debugger;
    var scopeoworkid = $('#WarrantyID').val();
    var content = tinymce.get('txtDescription2').getContent()

    var obj = {
        ID: scopeoworkid,
       
        Warrantytype: $('#drpwarrantytype').val(),
        EquipmentTypeID: $('#EquipmentTypeID').val(),
        Description: content,
        
    }

    // NOT stringified again
    // You can add AJAX here to save the changes to the server
    $.ajax({
        type: "POST",
        url: "/ProductType/SaveWarrantyItem1",
        contentType: "application/json;",
        dataType: "json",
        data: JSON.stringify(obj),

        //headers: { 'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() },
        success: function (data) {
            debugger;
            console.log(data);
            $('#scopepopup').modal('hide');
            $.ajax({
                type: "POST",
                url: '/ProductType/ShowWarranty/',
                datatype: "html",
                data: { EquipmentTypeID: $('#EquipmentTypeID').val() },
                success: function (data) {
                    $("#DetailContainer2").html(data);
                    $('#txtDescription2').val('');
                    $('#warrantypopup').modal('hide');
                }
            });

        }
    });
}

function SaveExclusionPopup() {
    debugger;
    var scopeoworkid = $('#ExclusionID').val();
    var content = tinymce.get('txtDescription3').getContent()

    var obj = {
        ID: scopeoworkid,        
        EquipmentTypeID: $('#EquipmentTypeID').val(),
        Description: content,
        //OrderNo: $('#txtOrderNo1').val()
    }

    // NOT stringified again
    // You can add AJAX here to save the changes to the server
    $.ajax({
        type: "POST",
        url: "/ProductType/SaveExclusionItem1",
        contentType: "application/json;",
        dataType: "json",
        data: JSON.stringify(obj),

        //headers: { 'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() },
        success: function (data) {
            debugger;
            console.log(data);
            $('#exclusionpopup').modal('hide');
            $.ajax({
                type: "POST",
                url: '/ProductType/ShowExclusion/',
                datatype: "html",
                data: { EquipmentTypeID: $('#EquipmentTypeID').val() },
                success: function (data1) {
                    $("#DetailContainer3").html(data1);
                   
                    $('#exclusionpopup').modal('hide');
                }
            });

        }
    });
}


function DeleteScopeDetailEntry(index) {
    debugger;
    Swal.fire({ title: "Are you sure?", text: "You won't be able to revert this!", icon: "warning", showCancelButton: !0, confirmButtonColor: "#34c38f", cancelButtonColor: "#f46a6a", confirmButtonText: "Yes, delete it!" }).then(
        function (t) {
            if (t.value) {
                var scopeofworkid = $('#QScopeWorkID_' + index).val();
                $.ajax({
                    type: "POST",
                    url: '/ProductType/DeleteScopeEntry/',
                    datatype: "html",
                    data: { id: scopeofworkid },
                    success: function (data) {
                        if (data.status == "OK") {
                            $.ajax({
                                type: "POST",
                                url: '/ProductType/ShowScopeofWork/',
                                datatype: "html",
                                data: { EquipmentTypeID: $('#EquipmentTypeID').val() },
                                success: function (data) {
                                    $("#DetailContainer1").html(data);
                                }
                            });
                        }
                    }
                });
            }
        });

}

function DeleteWarrantyDetailEntry(index) {
    debugger;
    Swal.fire({ title: "Are you sure?", text: "You won't be able to revert this!", icon: "warning", showCancelButton: !0, confirmButtonColor: "#34c38f", cancelButtonColor: "#f46a6a", confirmButtonText: "Yes, delete it!" }).then(
        function (t) {
            if (t.value) {
                var scopeofworkid = $('#QWarrantyID_' + index).val();
                $.ajax({
                    type: "POST",
                    url: '/ProductType/DeleteWarrantyEntry/',
                    datatype: "html",
                    data: { id: scopeofworkid },
                    success: function (data) {
                        if (data.status == "OK") {
                            $.ajax({
                                type: "POST",
                                url: '/ProductType/ShowWarranty/',
                                datatype: "html",
                                data: { EquipmentTypeID: $('#EquipmentTypeID').val() },
                                success: function (data) {
                                    $("#DetailContainer2").html(data);
                                }
                            });
                        }
                    }
                });
            }
        });

}

function DeleteExclusionDetailEntry(index) {
    debugger;
    Swal.fire({ title: "Are you sure?", text: "You won't be able to revert this!", icon: "warning", showCancelButton: !0, confirmButtonColor: "#34c38f", cancelButtonColor: "#f46a6a", confirmButtonText: "Yes, delete it!" }).then(
        function (t) {
            if (t.value) {
                var scopeofworkid = $('#QExclusionID_' + index).val();
                $.ajax({
                    type: "POST",
                    url: '/ProductType/DeleteExclusionEntry/',
                    datatype: "html",
                    data: { id: scopeofworkid },
                    success: function (data) {
                        if (data.status == "OK") {
                            $.ajax({
                                type: "POST",
                                url: '/ProductType/ShowExclusion/',
                                datatype: "html",
                                data: { EquipmentTypeID: $('#EquipmentTypeID').val() },
                                success: function (data) {
                                    $("#DetailContainer3").html(data);
                                }
                            });
                        }
                    }
                });
            }
        });
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
                        debugger;
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
      
            $('.edit-btn').on('click', function () {
                var $row = $(this).closest('tr');

                $row.find('.editable').each(function () {
                    var _text = $(this).find('input');
                    var _span = $(this).find('span');
                    $(_text).attr('type', 'text');
                    $(_span).css('display', 'none');

                });

                $row.find('.edit-btn').hide();
                $row.find('.save-btn').show();
                $row.find('.editable input:first').focus().select();
            });

            $('.save-btn').on('click', function () {
                var $row = $(this).closest('tr');

                $row.find('.editable').each(function () {
                    var inputVal = $(this).find('input').val();
                    var _text = $(this).find('input');
                    var _span = $(this).find('span');
                    $(_text).attr('type', 'hidden');
                    $(_span).html(inputVal);
                    $(_span).css('display', 'block');

                });

                $row.find('.save-btn').hide();
                $row.find('.edit-btn').show();

                // You can add AJAX here to save the changes to the server
            });
      

    })


})(jQuery)
