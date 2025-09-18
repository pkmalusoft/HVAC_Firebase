 
function SaveScopeItem(obj) {
    debugger;

    var itemcount = $('#DetailTable1 > tbody > tr').length;

    $(obj).attr('disabled', 'disabled');

    var scopedetails = [];

    if (itemcount == 0) {
        Swal.fire('Data Validation', 'Add Scope of Work Items', 'Info');
        return;
    }
    else {
        for (i = 0; i < itemcount; i++) {
            var quotationdetail = {
                ID: $('#QScopeWorkID_' + i).val(),
                EquipmentTypeID: $('#EquipmentTypeID').val(),
                Model: $('#txtModel_'+i).val(),
                Description: $('#txtDescription1_' + i).val(),
                Deleted: $('#QScopeDeleted_' + i).val()
            }

            scopedetails.push(quotationdetail);
            if ((i + 1) == itemcount) {
                $.ajax({
                    type: "POST",
                    url: '/ProductType/SaveScopeItem/',
                    datatype: "html",
                    data: { EquipmentTypeID: $('#EquipmentTypeID').val(), Details: JSON.stringify(scopedetails) },
                    success: function (data) {
                        if (data.status == "OK") {
                            Swal.fire('Save Status', data.message, 'Success');
                            $(obj).removeAttr('disabled');
                            setTimeout(function () {
                                $.ajax({
                                    type: "POST",
                                    url: '/ProductType/ShowScopeofWork/',
                                    datatype: "html",
                                    data: { EquipmentTypeID: $('#EquipmentTypeID').val() },
                                    success: function (data) {
                                        $("#DetailContainer1").html(data);
                                        $(obj).removeAttr('disabled');
                                        $('#txtDescription1').val('');
                                        $('#txtDescription1').focus();
                                    }
                                });
                            }, 100)


                        }
                        else {
                            Swal.fire('Save Status', data.message, 'warning');
                            $(obj).removeAttr('disabled');
                        }
                    }
                });
            }
        }
    }
}

function SaveWarrantyItem(obj) {
    debugger;

    var itemcount = $('#DetailTable2 > tbody > tr').length;

    $(obj).attr('disabled', 'disabled');

    var scopedetails = [];

    if (itemcount == 0) {
        Swal.fire('Data Validation', 'Add Warranty Items', 'Info');
        return;
    }
    else {
        for (i = 0; i < itemcount; i++) {
            var quotationdetail = {
                ID: $('#QWarrantyID_' + i).val(),
                EquipmentTypeID: $('#EquipmentTypeID').val(),
                WarrantyType: $('#txtWarrantyType_' + i).val(),
                Description: $('#QtxtWarrantyDescription_' + i).val(),
                Deleted: $('#QWarrantyDeleted_' + i).val()
            }

            scopedetails.push(quotationdetail);
            if ((i + 1) == itemcount) {
                $.ajax({
                    type: "POST",
                    url: '/ProductType/SaveWarrantyItem/',
                    datatype: "html",
                    data: { EquipmentTypeID: $('#EquipmentTypeID').val(), Details: JSON.stringify(scopedetails) },
                    success: function (data) {
                        if (data.status == "OK") {
                            Swal.fire('Save Status', data.message, 'Success');
                            $(obj).removeAttr('disabled');
                            setTimeout(function () {
                                $.ajax({
                                    type: "POST",
                                    url: '/ProductType/ShowWarranty/',
                                    datatype: "html",
                                    data: { EquipmentTypeID: $('#EquipmentTypeID').val() },
                                    success: function (data) {
                                        $("#DetailContainer2").html(data);
                                        $(obj).removeAttr('disabled');
                                        $('#txtDescription2').val('');
                                        $('#txtDescription2').focus();
                                    }
                                });
                            }, 100)


                        }
                        else {
                            Swal.fire('Save Status', data.message, 'warning');
                            $(obj).removeAttr('disabled');
                        }
                    }
                });
            }
        }
    }
}


function SaveExclusionItem(obj) {
    debugger;

    var itemcount = $('#DetailTable3 > tbody > tr').length;

    $(obj).attr('disabled', 'disabled');

    var scopedetails = [];

    if (itemcount == 0) {
        Swal.fire('Data Validation', 'Add Exclusion Description', 'Info');
        return;
    }
    else {
        for (i = 0; i < itemcount; i++) {
            var quotationdetail = {
                ID: $('#QExclusionID_' + i).val(),
                EquipmentTypeID: $('#EquipmentTypeID').val(),
                Description: $('#QtxtExDescription_' + i).val(),
                Deleted: $('#QExclusionDeleted_' + i).val()
            }

            scopedetails.push(quotationdetail);
            if ((i + 1) == itemcount) {
                $.ajax({
                    type: "POST",
                    url: '/ProductType/SaveExclusion/',
                    datatype: "html",
                    data: { EquipmentTypeID: $('#EquipmentTypeID').val(), Details: JSON.stringify(scopedetails) },
                    success: function (data) {
                        if (data.status == "OK") {
                            Swal.fire('Save Status', data.message, 'Success');
                            $(obj).removeAttr('disabled');
                            setTimeout(function () {
                                $.ajax({
                                    type: "POST",
                                    url: '/ProductType/ShowExclusion/',
                                    datatype: "html",
                                    data: { EquipmentTypeID: $('#EquipmentTypeID').val() },
                                    success: function (data) {
                                        $("#DetailContainer3").html(data);
                                        $(obj).removeAttr('disabled');
                                        $('#txtDescription3').val('');
                                        $('#txtDescription3').focus();
                                    }
                                });
                            }, 100)


                        }
                        else {
                            Swal.fire('Save Status', data.message, 'warning');
                            $(obj).removeAttr('disabled');
                        }
                    }
                });
            }
        }
    }
}
function AddDetail1(obj) {
    debugger;

    var itemcount = $('#DetailTable1 > tbody > tr').length;

    $(obj).attr('disabled', 'disabled');

    
    var _description = $('#txtDescription1').val();
    var equipmententry = {
        ID: 0,
        EquipmentTypeID: $('#EquipmentTypeID').val(),
        Model: $('#txtModel').val(),
        Description: _description,
        Deleted: false
    }

    var scopedetails = [];

    if (itemcount == 0) {
        $.ajax({
            type: "POST",
            url: '/ProductType/AddScopeItem/',
            datatype: "html",
            data: { invoice: equipmententry, index: -1, Details: JSON.stringify(scopedetails) },
            success: function (data) {
                $("#DetailContainer1").html(data);
                $(obj).removeAttr('disabled');
                $('#txtDescription1').val('');
                $('#txtDescription1').focus();
            }
        });
    }
    else {
        for (i = 0; i < itemcount; i++) {


            var quotationdetail = {
                ID: $('#QScopeWorkID_' + i).val(),
                EquipmentTypeID: $('#EquipmentTypeID').val(),
                Model: $('#txtModel_'+i).val(),
                Description: $('#txtDescription1_' + i).val(),
                Deleted: $('#QScopeDeleted_' + i).val()
            }

            scopedetails.push(quotationdetail);
            if ((i + 1) == itemcount) {
                $.ajax({
                    type: "POST",
                    url: '/ProductType/AddScopeItem/',
                    datatype: "html",
                    data: { invoice: equipmententry, index: -1, Details: JSON.stringify(scopedetails) },
                    success: function (data) {
                        $("#DetailContainer1").html(data);
                        $(obj).removeAttr('disabled');
                        $('#txtDescription1').val('');
                        $('#txtDescription1').focus();
                    }
                });
            }
        }
    }
}

function AddDetail2(obj) {
    debugger;

    var itemcount = $('#DetailTable2 > tbody > tr').length;

    $(obj).attr('disabled', 'disabled');


    var _description = $('#txtDescription2').val();
    var _entry = {
        ID: 0,
        EquipmentTypeID: $('#EquipmentTypeID').val(),
        WarrantyType: $('#drpwarrantytype').val(),
        Description: _description,
        Deleted: false
    }

    var warrantydetails = [];
    if (itemcount == 0) {
        $.ajax({
            type: "POST",
            url: '/ProductType/AddWarrantyItem/',
            datatype: "html",
            data: { invoice: _entry, index: -1, Details: JSON.stringify(warrantydetails) },
            success: function (data) {
                $("#DetailContainer2").html(data);
                $(obj).removeAttr('disabled');
                $('#txtDescription2').val('');
                $('#txtDescription2').focus();
            }
        });

    }
    else {
        for (i = 0; i < itemcount; i++) {

            var warrantydetail = {
                ID: $('#QWarrantyID_' + i).val(),
                EquipmentTypeID: $('#EquipmentTypeID').val(),
                WarrantyType: $('#txtWarrantyType_'+i).val(),
                Description: $('#QtxtWarrantyDescription_' + i).val(),
                Deleted: $('#QWarrantyDeleted_' + i).val()
            }

            warrantydetails.push(warrantydetail);

            if (itemcount == (i + 1)) {
                $.ajax({
                    type: "POST",
                    url: '/ProductType/AddWarrantyItem/',
                    datatype: "html",
                    data: { invoice: _entry, index: -1, Details: JSON.stringify(warrantydetails) },
                    success: function (data) {
                        $("#DetailContainer2").html(data);
                        $(obj).removeAttr('disabled');
                        $('#txtDescription2').val('');
                        $('#txtDescription2').focus();
                    }
                });
            }
        }

    }
}

function AddDetail3(obj) {
    debugger;

    var itemcount = $('#DetailTable3 > tbody > tr').length;

    $(obj).attr('disabled', 'disabled');


    var _description = $('#txtDescription3').val();
    var _entry = {
        ID: 0,
        EquipmentTypeID: $('#EquipmentTypeID').val(),
        Description: _description,
        Deleted: false
    }

    var exclusiondetails = [];
    if (itemcount == 0) {
        $.ajax({
            type: "POST",
            url: '/ProductType/AddExclusionItem/',
            datatype: "html",
            data: { invoice: _entry, index: -1, Details: JSON.stringify(exclusiondetails) },
            success: function (data) {
                $("#DetailContainer3").html(data);
                $(obj).removeAttr('disabled');
                $('#txtDescription3').val('');
                $('#txtDescription3').focus();
            }
        });

    }
    else {
        for (i = 0; i < itemcount; i++) {

            var exclusiondetail = {
                ID: $('#QExclusionID_' + i).val(),
                EquipmentTypeID: $('#EquipmentTypeID').val(),
                Description: $('#QtxtExDescription_' + i).val(),
                Deleted: $('#QExclusionDeleted_' + i).val()
            }

            exclusiondetails.push(exclusiondetail);

            if (itemcount == (i + 1)) {
                $.ajax({
                    type: "POST",
                    url: '/ProductType/AddExclusionItem/',
                    datatype: "html",
                    data: { invoice: _entry, index: -1, Details: JSON.stringify(exclusiondetails) },
                    success: function (data) {
                        $("#DetailContainer3").html(data);
                        $(obj).removeAttr('disabled');
                        $('#txtDescription3').val('');
                        $('#txtDescription3').focus();
                    }
                });
            }
        }

    }
}



function DeleteScopeDetailEntry(index) {
    $('#QScopeDeleted_' + index).val('true');
    $('#QScopeDeleted_' + index).parent().parent().addClass('hide');
    var scopedetails = [];
    var itemcount = $('#DetailTable1 > tbody > tr').length;
    for (i = 0; i < itemcount; i++) {


        var quotationdetail = {
            ID: $('#QScopeWorkID_' + i).val(),
            EquipmentTypeID: $('#EquipmentTypeID').val(),
            Model: $('#txtModel_' + i).val(),
            Description: $('#txtDescription1_' + i).val(),
            Deleted: $('#QScopeDeleted_' + i).val()
        }

        scopedetails.push(quotationdetail);
        if ((i + 1) == itemcount) {
            $.ajax({
                type: "POST",
                url: '/ProductType/AddScopeItem/',
                datatype: "html",
                data: { invoice: null, index: -1, Details: JSON.stringify(scopedetails) },
                success: function (data) {
                    $("#DetailContainer1").html(data);
                   
                    $('#txtDescription1').val('');
                    $('#txtDescription1').focus();
                }
            });
        }
    }

}

function DeleteWarrantyDetailEntry(index) {
    $('#QWarrantyDeleted_' + index).val('true');
    $('#QWarrantyDeleted_' + index).parent().parent().addClass('hide');

    var itemcount = $('#DetailTable2 > tbody > tr').length;
    var warrantydetails = [];
    for (i = 0; i < itemcount; i++) {

        var warrantydetail = {
            ID: $('#QWarrantyID_' + i).val(),
            EquipmentTypeID: $('#EquipmentTypeID').val(),
            WarrantyType: $('#txtWarrantyType_'+i).val(),
            Description: $('#QtxtWarrantyDescription_' + i).val(),
            Deleted: $('#QWarrantyDeleted_' + i).val()
        }

        warrantydetails.push(warrantydetail);

        if (itemcount == (i + 1)) {
            $.ajax({
                type: "POST",
                url: '/ProductType/AddWarrantyItem/',
                datatype: "html",
                data: { invoice: null, index: -1, Details: JSON.stringify(warrantydetails) },
                success: function (data) {
                    $("#DetailContainer2").html(data);
                   
                    $('#txtDescription2').val('');
                    $('#txtDescription2').focus();
                }
            });
        }
    }

}

function DeleteExclusionDetailEntry(index) {
    $('#QExclusionDeleted_' + index).val('true');
    $('#QExclusionDeleted_' + index).parent().parent().addClass('hide');
    var exclusiondetails = [];
    var itemcount = $('#DetailTable3 > tbody > tr').length;
    for (i = 0; i < itemcount; i++) {

        var exclusiondetail = {
            ID: $('#QExclusionID_' + i).val(),
            EquipmentTypeID: $('#EquipmentTypeID').val(),
            Description: $('#QtxtExDescription_' + i).val(),
            Deleted: $('#QExclusionDeleted_' + i).val()
        }

        exclusiondetails.push(exclusiondetail);

        if (itemcount == (i + 1)) {
            $.ajax({
                type: "POST",
                url: '/ProductType/AddExclusionItem/',
                datatype: "html",
                data: { invoice: null, index: -1, Details: JSON.stringify(exclusiondetails) },
                success: function (data) {
                    $("#DetailContainer3").html(data);
                   
                    $('#txtDescription3').val('');
                    $('#txtDescription3').focus();
                }
            });
        }
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
