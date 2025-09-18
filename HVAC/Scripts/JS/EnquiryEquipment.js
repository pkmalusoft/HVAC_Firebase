 
 
function clearEquipment() {
     
    $('#EquipmentID').val(0);
    $('#eEnquiryID').val(0);
    $('#eEquipmentTypeID').val(0);
     $('#ProductFamilyID').val('');
    $('#BrandID').val(0);
    $('#eEquipmentType').val('');        
    $('#BrandID').val(0);
    $('#eBrand').val('');
        
    
}

function setdocumentrowactive(index1) {
    var idtext = 'doctr_'
    $('[id^=' + idtext + ']').each(function (index, item) {
        $('#doctr_' + index).removeClass('rowactive');
    });
    $('#doctr_' + index1).addClass('rowactive');
}
 

function SaveEquipmententry() {
    debugger;
   

    if ($('#EquipmentCategoryID').val() == '') {
      
        $('#spanequiperror').html('Select Equipment Category!');
        $('#EquipmentCategoryID').focus();
        return;
    }
    else if ($('#EquipmentName').val() == '' || $('#EquipmentName').val() == '0') {
         
        $('#spanequiperror').html('Enter Equipment Name!');
        $('##EquipmentName').focus();
        return;
    }
    
    $('#btnSaveEquipment').attr('disabled', 'disabled');
    debugger;
    var _equipmentname = "";//$('#ProductFamilyID option:selected').text();
    _equipmentname = $('#eEquipmentType option:selected').text(); // + '/' + $('#eBrand').val();// + '/' + $('#eModel').val();
    var _brandname = $('#BrandID option:selected').text();
    var equipmententry = {
        ID: $('#EquipmentID').val(),
        EnquiryID: $('#eEnquiryID').val(),
        EquipmentTypeID: $('#eEquipmentType').val(),
        ProductFamilyID: $('#ProductFamilyID').val(),
        BrandID: $('#BrandID').val(),
        EquipmentName: _equipmentname,
        BrandID: $('#BrandID').val(),
        Brand: _brandname,
        Model: '', //$('#eModel').val(),        
        Quantity: 0, //$('#eQuantity').val(),
        UnitRate: 0,//$('#eUnitRate').val(),
        Amount: 0, //$('#eAmount').val(),
        EquipmentStatusID: 1, //$('#eEquipmentStatusID').val()
        EnquiryNo :$('#EnquiryNo').val()
    }

    $.ajax({
        type: "POST",
        url: '/Enquiry/SaveEquipment/',
        datatype: "json",
        data: equipmententry,
        success: function (response) {
            debugger;
            if (response.status == "OK") {
            
                Swal.fire({
                    title: "Save Status",
                    text: response.message,
                    icon: "success",
                    timer: 200
                    //showCancelButton: true,
                    //confirmButtonText: "Yes, delete it!"
                });
                $('#equipmentpopup').modal('hide');
                $('#btnSaveEquipment').removeAttr('disabled');
                reloadEquipment();
                reloadlog();
            }
            else {
                Swal.fire({
                    title: "Save Status",
                    text: response.message,
                    icon: "warning",
                    timer: 200
                    //showCancelButton: true,
                    //confirmButtonText: "Yes, delete it!"
                });
               
            }
            

        }
    });
}
function reloadEquipment() {
    $.ajax({
        type: "POST",
        url: '/Enquiry/ListEquipment',
        datatype: "html",
        data: { id: $('#EnquiryID').val() },
        success: function (data) {
            debugger;
            $("#divequipment").html(data);
            //$("#divEquipmentContainer").html(data);
            clearEquipment();
            //$('#btnSaveEquipment').removeAttr('disabled');            
        }
    });
}

function EditEquipment(id, index1) {

    debugger;
    $.ajax({
        type: 'POST',
        url: '/Enquiry/ShowEquipmentEntry',
        datatype: "html",
        data: {
            id: id, EnquiryID: $('#EnquiryID').val()
        },
        success: function (data) {
            $("#equipmentContainer").html(data);
            //target
            $('#equipmentpopup').modal('show');
            $('#spanequipmenterror').html("");
            setTimeout(function () {
                //$('#EquipmentCategoryID').focus();
                $('#BrandID').focus();
            }, 200)

        }
    });
     
}

function DeleteEquipment(EquipmentId, index1) {
    setdocumentrowactive(index1);
    Swal.fire({ title: "Are you sure?", text: "You won't be able to revert this!", icon: "warning", showCancelButton: !0, confirmButtonColor: "#34c38f", cancelButtonColor: "#f46a6a", confirmButtonText: "Yes, delete it!" }).then(
        function (t) {
            if (t.value) {
                $.ajax({
                    type: "POST",
                    url: '/Enquiry/DeleteEquipment',
                    datatype: "html",
                    data: {
                        'EquipmentId': EquipmentId, 'EnquiryID': $('#EnquiryID').val()
                    },
                    success: function (data) {
                        // alert(data);
                        $("#divequipment").html(data);

                       
                        

                    }
                });
            }
        });
    
     
}
 
(function () {
    "use strict";
    window.addEventListener(
        "load",
        function () {
            var t = document.getElementsByClassName("documentneeds-validation");
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

                            SaveEquipment();

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

       
        $('#equipmentpopup').on('shown.bs.modal', function () {
            $('#BrandID').focus();
        });

      
    });
