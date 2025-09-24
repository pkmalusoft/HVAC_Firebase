
function SaveScopeItem(obj) {
    debugger;

    var itemcount = 2; //$('#DetailTable1 > tbody > tr').length;

    $(obj).attr('disabled', 'disabled');

    var scopedetails = [];

    if (itemcount == 0) {
        Swal.fire('Data Validation', 'Add Scope of Work Items', 'Info');
        return;
    }
    else {
        for (si = 0; si < itemcount; si++) {
            var quotationdetail = {
                ID: $('#QScopeWorkID_' + si).val(),
                QuotationID: $('#QuotationID').val(),
                EquipmentID: $('#QScopeEquipmentID_' + si).val(),
                EquipmentName: $('#txtEquipmentName1_' + si).val(),
                Model: $('#txtModel_' + si).val(),
                Description: $('#txtDescription1_' + si).val(),
                OrderNo: $('#txtOrderNo1_' + si).val(),
                Checked: $('#chkScopeItem_' + si).prop('checked')
            }

            scopedetails.push(quotationdetail);
            if ((si + 1) == itemcount) {
                $.ajax({
                    type: "POST",
                    url: '/Quotation/SaveScopeItem/',
                    contentType: 'application/json',
                    dataType: "json",
                    data: { QuotationID: $('#QuotationID').val(), Details: JSON.stringify(scopedetails) },
                    // include anti-forgery if you use it:
                    //headers: { 'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() },
                    success: function (data) {
                        if (data.status == "OK") {
                            Swal.fire('Save Status', data.message, 'Success');
                            $(obj).removeAttr('disabled');
                            setTimeout(function () {
                                $.ajax({
                                    type: "POST",
                                    url: '/Quotation/ShowScopeofWork/',
                                    datatype: "html",
                                    data: { QuotationID: $('#QuotationID').val() },
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
                QuotationID: $('#QuotationID').val(),
                EquipmentID: $('#QWarrantyEquipmentID_' + i).val(),
                EquipmentName: $('#txtEquipmentName2_' + i).val(),
                WarrantyType: $('#txtWarranty_' + i).val(),
                Description: $('#QtxtWarrantyDescription_' + i).val(),
                Deleted: $('#QWarrantyDeleted_' + i).val()
            }

            scopedetails.push(quotationdetail);
            if ((i + 1) == itemcount) {
                $.ajax({
                    type: "POST",
                    url: '/Quotation/SaveWarrantyItem/',
                    datatype: "html",
                    data: { QuotationID: $('#QuotationID').val(), Details: JSON.stringify(scopedetails) },
                    success: function (data) {
                        if (data.status == "OK") {
                            Swal.fire('Save Status', data.message, 'Success');
                            $(obj).removeAttr('disabled');
                            setTimeout(function () {
                                $.ajax({
                                    type: "POST",
                                    url: '/Quotation/ShowWarranty/',
                                    datatype: "html",
                                    data: { QuotationID: $('#QuotationID').val() },
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
function openSubjectModal() {
    let currentValue = document.getElementById("QtxtSubject").value;
    console.log("Current Value from QtxtSubject:", currentValue); // ✅ print in console

    // Wait until editor is ready
    if (tinymce.get("subjectModalTextarea")) {
        tinymce.get("subjectModalTextarea").setContent(currentValue || "");
        console.log("Value set in TinyMCE:", tinymce.get("subjectModalTextarea").getContent()); // ✅ print editor value
    }

    var modal = new bootstrap.Modal(document.getElementById('subjectModal'));
    modal.show();
}
function saveSubjectModal() {
    let newValue = "";

    // get value from TinyMCE editor if active
    if (tinymce.get("subjectModalTextarea")) {
        newValue = tinymce.get("subjectModalTextarea").getContent();
        console.log("Saved string:", newValue);
    } else {
        newValue = document.getElementById("subjectModalTextarea").value;
    }

    // update hidden textbox and display element
    document.getElementById("QtxtSubject").value = newValue.toString();
    document.getElementById("subjectDisplay").innerHTML = newValue.toString();

    // close modal
    var modalElement = document.getElementById('subjectModal');
    var modal = bootstrap.Modal.getInstance(modalElement);
    modal.hide();
}

function openValidityPopup() {
    let currentValue = document.getElementById("QtxtValidity").value;
    if (tinymce.get("txtValidity")) {
        tinymce.get("txtValidity").setContent(currentValue || "");
    }

    var modal = new bootstrap.Modal(document.getElementById('validityModal'));
    modal.show();

}

function saveValidityPopup() {
    let newValue = "";

    if (tinymce.get("txtValidity")) {
        newValue = tinymce.get("txtValidity").getContent();
    } else {
        newValue = document.getElementById("txtValidity").value;
    }

    document.getElementById("QtxtValidity").value = newValue;
    document.getElementById("validityDisplay").innerHTML = newValue;

    var modalElement = document.getElementById('validityModal');
    var modal = bootstrap.Modal.getInstance(modalElement);
    modal.hide();

}
function openDeliveryTermsModal() {
    let currentValue = document.getElementById("QtxtDeliveryTerms").value;

    if (tinymce.get("deliveryTermsModalTextarea")) {
        tinymce.get("deliveryTermsModalTextarea").setContent(currentValue || "");
    }
    var modal = new bootstrap.Modal(document.getElementById('deliveryTermsModal'));
    modal.show();

}
function openTermsModal() {
    let currentValue = document.getElementById("QtxtTermsCondition").value;

    if (tinymce.get("TermsConditionTextarea")) {
        tinymce.get("TermsConditionTextarea").setContent(currentValue || "");
    }
    var modal = new bootstrap.Modal(document.getElementById('termsconditionModal'));
    modal.show();

}


function saveDeliveryTermsModal() {
    let newValue = "";

    if (tinymce.get("deliveryTermsModalTextarea")) {
        newValue = tinymce.get("deliveryTermsModalTextarea").getContent();
    } else {
        newValue = document.getElementById("deliveryTermsModalTextarea").value;
    }

    document.getElementById("QtxtDeliveryTerms").value = newValue;
    document.getElementById("deliveryTermsDisplay").innerHTML = newValue;

    var modalElement = document.getElementById('deliveryTermsModal');
    var modal = bootstrap.Modal.getInstance(modalElement);
    modal.hide();
}
function saveTermsModal() {
    let newValue = "";

    if (tinymce.get("TermsConditionTextarea")) {
        newValue = tinymce.get("TermsConditionTextarea").getContent();
    } else {
        newValue = document.getElementById("TermsConditionTextarea").value;
    }

    document.getElementById("QtxtTermsCondition").value = newValue;
    document.getElementById("TermsConditionDisplay").innerHTML = newValue;

    var modalElement = document.getElementById('termsconditionModal');
    var modal = bootstrap.Modal.getInstance(modalElement);
    modal.hide();
}

/*New Popup Items Working */
function AddScopePopup(ID) {
    debugger;
    $.ajax({
        type: "POST",
        url: '/Quotation/ShowAddScopeofWork/',
        datatype: "html",
        data: { QuotationID: $('#QuotationID').val(), EnquiryID: $('#EnquiryID').val(), ID: ID },
        success: function (data) {
            $("#scopeContainer").html(data);
            $('#scopepopup').modal('show');
        }
    });
}

function AddWarranty(ID) {
    $.ajax({
        type: "POST",
        url: '/Quotation/ShowAddWarranty/',
        datatype: "html",
        data: { QuotationID: $('#QuotationID').val(), EnquiryID: $('#EnquiryID').val(), ID: ID },
        success: function (data) {
            $("#warrantyContainer").html(data);
            $('#warrantypopup').modal('show');
        }
    });
}

function AddExclusion(ID) {
    $.ajax({
        type: "POST",
        url: '/Quotation/ShowAddExclusion/',
        datatype: "html",
        data: { QuotationID: $('#QuotationID').val(), EnquiryID: $('#EnquiryID').val(), ID: ID },
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
        QuotationID: $('#QuotationID').val(),
        Model: $('#txtModel1').val(),
        EquipmentID: $('#drpEquipment1').val(),
        Description: content,
        OrderNo: $('#txtOrderNo1').val(),
        Airmech: $('#ChkResAirmech').prop('checked'),
        client: $('#ChkResClient').prop('checked')
    }

    // NOT stringified again
    // You can add AJAX here to save the changes to the server
    $.ajax({
        type: "POST",
        url: "/Quotation/SaveScopeItem1",
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
                url: '/Quotation/ShowScopeofWork/',
                datatype: "html",
                data: { QuotationID: $('#QuotationID').val() },
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
    var scopeoworkid = $('#warrantyid').val();
    var content = tinymce.get('txtDescription2').getContent()

    var obj = {
        ID: scopeoworkid,
        QuotationID: $('#QuotationID').val(),
        Warrantytype: $('#drpwarrantytype').val(),
        EquipmentID: $('#drpEquipment2').val(),
        Description: content,
        //OrderNo: $('#txtOrderNo1').val()
    }

    // NOT stringified again
    // You can add AJAX here to save the changes to the server
    $.ajax({
        type: "POST",
        url: "/Quotation/SaveWarrantyItem1",
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
                url: '/Quotation/ShowWarranty/',
                datatype: "html",
                data: { QuotationID: $('#QuotationID').val() },
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
    var scopeoworkid = $('#exclusionid').val();
    var content = tinymce.get('txtDescription3').getContent()

    var obj = {
        ID: scopeoworkid,
        QuotationID: $('#QuotationID').val(),
        //Model: $('#txtModel1').val(),
        EquipmentID: $('#drpEquipment3').val(),
        Description: content,
        //OrderNo: $('#txtOrderNo1').val()
    }

    // NOT stringified again
    // You can add AJAX here to save the changes to the server
    $.ajax({
        type: "POST",
        url: "/Quotation/SaveExclusionItem1",
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
                url: '/Quotation/ShowExclusion/',
                datatype: "html",
                data: { QuotationID: $('#QuotationID').val() },
                success: function (data) {
                    $("#DetailContainer3").html(data);
                    $('#txtDescription3').val('');
                    $('#exclusionpopup').modal('hide');
                }
            });

        }
    });
}
function AddDetail1(obj) {
    debugger;

    var itemcount = $('#DetailTable1 > tbody > tr').length;

    $(obj).attr('disabled', 'disabled');


    var _description = $('#txtDescription1').val();
    var equipmententry = {
        ID: 0,
        QuotationID: $('#QuotationID').val(),
        EquipmentID: $('#drpEquipment1').val(),
        Model: $('#txtModel').val(),
        EquipmentName: $('#drpEquipment1').select2('data')[0]?.text,
        Description: _description,
        Checked: true
    }



    // NOT stringified again
    // You can add AJAX here to save the changes to the server
    $.ajax({
        type: "POST",
        url: "/Quotation/SaveScopeItem1",
        contentType: "application/json;",
        dataType: "json",
        data: JSON.stringify(equipmententry),

        //headers: { 'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() },
        success: function (data) {
            debugger;
            console.log(data);
            if (data.status == "OK") {
                $.ajax({
                    type: "POST",
                    url: '/Quotation/ShowScopeofWork/',
                    datatype: "html",
                    data: { QuotationID: $('#QuotationID').val() },
                    success: function (data) {
                        $("#DetailContainer1").html(data);
                        $(obj).removeAttr('disabled');
                        $('#txtDescription1').val('');
                        $('#txtDescription1').focus();
                    }
                });
            }
        }
    });

}

function AddMasterDetail1(obj) {
    debugger;

    var itemcount = $('#DetailTable1 > tbody > tr').length;

    $(obj).attr('disabled', 'disabled');


    var _description = $('#txtDescription1').val();
    var _equipmentname = $('#drpEquipment1').select2('data')[0]?.text;
    var scopedetails = [];

    if (itemcount == 0) {
        $.ajax({
            type: "POST",
            url: '/Quotation/AddMasterScopeofWork/',
            datatype: "html",
            data: { QuotationId: $('#QuotationID').val(), EquipmentId: $('#drpEquipment1').val(), EquipmentName: _equipmentname, Model: $('#txtModel').val(), Details: JSON.stringify(scopedetails) },
            success: function (data) {
                $("#DetailContainer1").html(data);
                $(obj).removeAttr('disabled');
                $('#txtModel').val('');
                $('#txtDescription1').val('');
                $('#txtModel').focus();
            }
        });
    }
    else {
        for (i = 0; i < itemcount; i++) {

            if ((i + 1) == itemcount) {
                $.ajax({
                    type: "POST",
                    url: '/Quotation/AddMasterScopeofWork/',
                    datatype: "html",
                    data: { QuotationId: $('#QuotationID').val(), EquipmentId: $('#drpEquipment1').val(), EquipmentName: _equipmentname, Model: $('#txtModel').val(), Details: JSON.stringify(scopedetails) },
                    success: function (data) {
                        $("#DetailContainer1").html(data);
                        $(obj).removeAttr('disabled');
                        $('#txtModel').val('');
                        $('#txtDescription1').val('');
                        $('#txtModel').focus();
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
        QuotationID: $('#QuotationID').val(),
        EquipmentID: $('#drpEquipment2').val(),
        WarrantyType: $('#drpwarrantytype').val(),
        EquipmentName: $('#drpEquipment2').select2('data')[0]?.text,
        Description: _description,
        Checked: true
    }

    var warrantydetails = [];
    if (itemcount == 0) {
        $.ajax({
            type: "POST",
            url: '/Quotation/AddWarrantyItem/',
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
                QuotationID: $('#QuotationID').val(),
                EquipmentID: $('#QWarrantyEquipmentID_' + i).val(),
                EquipmentName: $('#txtEquipmentName2_' + i).val(),
                WarrantyType: $('#txtWarranty_' + i).val(),
                Description: $('#QtxtWarrantyDescription_' + i).val(),
                Checked: $('#chkWarranty_' + i).prop('checked')
            }

            warrantydetails.push(warrantydetail);

            if (itemcount == (i + 1)) {
                $.ajax({
                    type: "POST",
                    url: '/Quotation/AddWarrantyItem/',
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

function AddMasterDetail2(obj) {
    debugger;

    var itemcount = $('#DetailTable2 > tbody > tr').length;

    $(obj).attr('disabled', 'disabled');


    var _description = $('#txtDescription2').val();
    var _entry = {
        ID: 0,
        QuotationID: $('#QuotationID').val(),
        EquipmentID: $('#drpEquipment2').val(),
        WarrantyType: $('#drpwarrantytype').val(),
        EquipmentName: $('#drpEquipment2').select2('data')[0]?.text,
        Description: _description,
        Checked: true
    }

    var warrantydetails = [];
    if (itemcount == 0) {
        $.ajax({
            type: "POST",
            url: '/Quotation/AddWarrantyItem/',
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
                QuotationID: $('#QuotationID').val(),
                EquipmentID: $('#QWarrantyEquipmentID_' + i).val(),
                EquipmentName: $('#txtEquipmentName2_' + i).val(),
                Description: $('#QtxtWarrantyDescription_' + i).val(),
                Checked: $('#chkWarranty_' + i).prop('checked')
            }

            warrantydetails.push(warrantydetail);

            if (itemcount == (i + 1)) {
                $.ajax({
                    type: "POST",
                    url: '/Quotation/AddWarrantyItem/',
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
        QuotationID: $('#QuotationID').val(),
        EquipmentID: $('#drpEquipment3').val(),
        EquipmentName: $('#drpEquipment3').select2('data')[0]?.text,
        Description: _description,
        Checked: true
    }

    var exclusiondetails = [];
    if (itemcount == 0) {
        $.ajax({
            type: "POST",
            url: '/Quotation/AddExclusionItem/',
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
                QuotationID: $('#QuotationID').val(),
                EquipmentID: $('#QExEquipmentID_' + i).val(),
                EquipmentName: $('#txtEquipmentName3_' + i).val(),
                Description: $('#QtxtExDescription_' + i).val(),
                Deleted: $('#QExclusionDeleted_' + i).val()
            }

            exclusiondetails.push(exclusiondetail);

            if (itemcount == (i + 1)) {
                $.ajax({
                    type: "POST",
                    url: '/Quotation/AddExclusionItem/',
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
    debugger;
    Swal.fire({ title: "Are you sure?", text: "You won't be able to revert this!", icon: "warning", showCancelButton: !0, confirmButtonColor: "#34c38f", cancelButtonColor: "#f46a6a", confirmButtonText: "Yes, delete it!" }).then(
        function (t) {
            if (t.value) {
                var scopeofworkid = $('#QScopeWorkID_' + index).val();
                $.ajax({
                    type: "POST",
                    url: '/Quotation/DeleteScopeEntry/',
                    datatype: "html",
                    data: { id: scopeofworkid },
                    success: function (data) {
                        if (data.status == "OK") {
                            $.ajax({
                                type: "POST",
                                url: '/Quotation/ShowScopeofWork/',
                                datatype: "html",
                                data: { QuotationID: $('#QuotationID').val() },
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
                    url: '/Quotation/DeleteWarrantyEntry/',
                    datatype: "html",
                    data: { id: scopeofworkid },
                    success: function (data) {
                        if (data.status == "OK") {
                            $.ajax({
                                type: "POST",
                                url: '/Quotation/ShowWarranty/',
                                datatype: "html",
                                data: { QuotationID: $('#QuotationID').val() },
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

function DeleteExclusionEntry(index) {
    debugger;
    Swal.fire({ title: "Are you sure?", text: "You won't be able to revert this!", icon: "warning", showCancelButton: !0, confirmButtonColor: "#34c38f", cancelButtonColor: "#f46a6a", confirmButtonText: "Yes, delete it!" }).then(
        function (t) {
            if (t.value) {
                var scopeofworkid = $('#QExclusionID_' + index).val();
                $.ajax({
                    type: "POST",
                    url: '/Quotation/DeleteExclusionEntry/',
                    datatype: "html",
                    data: { id: scopeofworkid },
                    success: function (data) {
                        if (data.status == "OK") {
                            $.ajax({
                                type: "POST",
                                url: '/Quotation/ShowExclusion/',
                                datatype: "html",
                                data: { QuotationID: $('#QuotationID').val() },
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

function DeleteWarrantyDetailEntryold(index) {
    $('#QWarrantyDeleted_' + index).val('true');
    $('#QWarrantyDeleted_' + index).parent().parent().addClass('hide');

    var itemcount = $('#DetailTable2 > tbody > tr').length;
    var warrantydetails = [];
    for (i = 0; i < itemcount; i++) {

        var warrantydetail = {
            ID: $('#QWarrantyID_' + i).val(),
            QuotationID: $('#QuotationID').val(),
            EquipmentID: $('#QWarrantyEquipmentID_' + i).val(),
            EquipmentName: $('#txtEquipmentName2_' + i).val(),
            Description: $('#QtxtWarrantyDescription_' + i).val(),
            Deleted: $('#QWarrantyDeleted_' + i).val()
        }

        warrantydetails.push(warrantydetail);

        if (itemcount == (i + 1)) {
            $.ajax({
                type: "POST",
                url: '/Quotation/AddWarrantyItem/',
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
            QuotationID: $('#QuotationID').val(),
            EquipmentID: $('#QExEquipmentID_' + i).val(),
            EquipmentName: $('#txtEquipmentName3_' + i).val(),
            Description: $('#QtxtExDescription_' + i).val(),
            Deleted: $('#QExclusionDeleted_' + i).val()
        }

        exclusiondetails.push(exclusiondetail);

        if (itemcount == (i + 1)) {
            $.ajax({
                type: "POST",
                url: '/Quotation/AddExclusionItem/',
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
function SaveContacts(obj) {
    debugger;

    var itemcount = $('#DetailTable4 > tbody > tr').length;

    $(obj).attr('disabled', 'disabled');

    var contactdetails = [];

    if (itemcount == 0) {
        Swal.fire('Data Validation', 'Add Exclusion Description', 'Info');
        return;
    }
    else {
        for (i = 0; i < itemcount; i++) {
            var contactdetail = {
                ID: $('#QCContactID_' + i).val(),
                QuotationID: $('#QCQuotationID').val(),
                ClientID: $('#QCClientID_' + i).val(),
                ContactName: $('#QCContactName_' + i).val(),
                EmailID: $('#QCEmailID_' + i).val(),
                PhoneNo: $('#QCPhoneNo_' + i).val()
            }

            contactdetails.push(contactdetail);
            if ((i + 1) == itemcount) {
                $.ajax({
                    type: "POST",
                    url: '/Quotation/SaveContacts/',
                    datatype: "html",
                    data: { QuotationID: $('#QuotationID').val(), Details: JSON.stringify(contactdetails) },
                    success: function (data) {
                        if (data.status == "OK") {
                            Swal.fire('Save Status', data.message, 'Success');
                            $(obj).removeAttr('disabled');
                            $('#contactpopup').modal('hide');


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




(function ($) {

    'use strict';
    function initformControl() {
        var suppressOpenOnClear = false;
        //$('.equipment-select').each(function () {
        //    var $select = $(this);

        //    // Handle the clear event
        //    $select.on('select2:select2-selection__arrow', function () {
        //        suppressOpenOnClear = true;

        //        // Remove focus from both the hidden <select> and visible Select2 box
        //        setTimeout(() => {
        //            $select.blur();
        //            $select.next('.select2-container').find('.select2-selection').blur();
        //        }, 0);
        //    });

        //    // Prevent dropdown from reopening immediately after clear
        //    $select.on('select2:opening', function (e) {
        //        if (suppressOpenOnClear) {
        //            e.preventDefault(); // Stop it from opening
        //            suppressOpenOnClear = false;
        //        }
        //    });
        //    var defaultOption = new Option($select.attr('boxname'), $select.attr('boxid'), true, true);
        //    $select.append(defaultOption);
        //    //// Append it to the select
        //    $select.append('<option id="" value=""></option>').trigger('change');
        //    $select.select2({
        //        placeholder: 'Select a product',
        //        allowClear: false,
        //        minLength: 1,
        //        ajax: {
        //            url: '/Estimation/GetEquipmentType',
        //            dataType: 'json',
        //            delay: 250,
        //            data: function (params) {
        //                // var category = $select.closest('tr').find('.category').val();
        //                return {
        //                    term: params.term,
        //                    EnquiryID: $('#EnquiryID').val(),
        //                    CategoryID: 1
        //                };
        //            },
        //            processResults: function (data) {
        //                return {
        //                    results: data.map(function (item) {
        //                        return { id: item.ID, text: item.Text };
        //                    })
        //                };
        //            },
        //            cache: true
        //        }
        //    });



        //});
    }

    function init() {
        initformControl();
    }
    window.addEventListener(
        "load",
        function () {
            var t = document.getElementsByClassName("needs-validation222");
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
        $('#btnshowcontact').click(function () {
            $.ajax({
                type: "POST",
                url: '/Quotation/GetContactDetail/',
                datatype: "html",
                data: { QuotationID: $('#QuotationID').val() },
                success: function (data) {
                    $("#quotationcontactContainer").html(data);

                    $('#contactpopup').modal('show');
                }
            });

        })
        //
        $("#txtDescription2").autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: '/Quotation/GetWarrantyItem',
                    datatype: "json",
                    data: {
                        'term': request.term, 'EquipmentId': $('#drpEquipment2').val(), 'Type': $('#drpwarrantytype').val()
                    },
                    success: function (data) {
                        response($.map(data, function (val, item) {
                            return {
                                label: val.Description,
                                value: val.Description
                            }
                        }))
                    }
                })
            },
            minLength: 1,
            autoFocus: false,
            focus: function (e, ui) {
                e.preventDefault();
                $('#txtDescription2').val(ui.item.label);


            },
            select: function (e, ui) {
                e.preventDefault();
                $('#txtDescription2').val(ui.item.label);

            },

        });


        //
        $("#txtDescription3").autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: '/Quotation/GetEclusionItem',
                    datatype: "json",
                    data: {
                        'term': request.term, 'EquipmentId': $('#drpEquipment3').val()
                    },
                    success: function (data) {
                        response($.map(data, function (val, item) {
                            return {
                                label: val.Description,
                                value: val.Description
                            }
                        }))
                    }
                })
            },
            minLength: 1,
            autoFocus: false,
            focus: function (e, ui) {
                e.preventDefault();
                $('#txtDescription3').val(ui.item.label);


            },
            select: function (e, ui) {
                e.preventDefault();
                $('#txtDescription3').val(ui.item.label);

            },

        });
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
