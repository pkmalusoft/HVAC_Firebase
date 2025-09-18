function SaveCustomerEntry() {
    debugger;
    if ($('#CustomerName').val().trim() == '') {
        $('#spancusterror').html("Enter Customer Name!");
        $('#CustomerName').focus();
        return;
    }
    else if ($('#Phone').val().trim() == '') {
        $('#spancusterror').html("Enter Customer Phone No.!");
        $('#Phone').focus();
        return;
    }
    else if ($('#CityName').val().trim() == '') {
        $('#spancusterror').html("Enter City Name!");
        $('#CityName').focus();
        return;
    }
    else if ($('#CountryName').val().trim() == '') {
        $('#spancusterror').html("Enter Country Name!");
        $('#CountryName').focus();
        return;
    }
    $('#spancusterror').html("");

    var cust = {
        CustomerName: $('#CustomerName').val(),
        Address1: $('#Address1').val(),
        Address2: $('#Address2').val(),
        Address3: $('#Address3').val(),
        LocationName: $('#LocationName').val(),
        CityName: $('#CityName').val(),
        CountryName: $('#CountryName').val(),
        ContactPerson: $('#ContactPerson').val(),
        Phone: $('#Phone').val(),
        Mobile: $('#Mobile').val(),
        CustomerType: $('#CustomerType1').val()
    }

    $.ajax({
        type: 'POST',
        url: '/CustomerMaster/SaveCustomerEntry',
        datatype: "json",
        data: cust,
        success: function (response) {
            debugger;
            if (response.status == 'Ok') {

                $('#CustomerName').val(response.data.CustomerName);
                $('#customer').attr('customername', response.data.CustomerName)
                $('#customerpopup').modal('hide');
                $('#customer').trigger('change');
                Swal.fire("Save Status!", response.message, "success");
                $('#CustomerName').val(response.data.CustomerName);
                $('#CustomerID').val(response.data.CustomerID);
                $('#Mobile1').val(response.data.Mobile);
                $('#CusAddress').val(response.data.Address1);
                $('#Location').val(response.data.LocationName);
                $('#City').val(response.data.CityName);
                $('#Country').val(response.data.CountryName);




                //$.notify(response.message, "success");

            }
            else {
                //$('#customerpopup').modal('hide');
                //$('#customer').val(response.data.CustomerName);
                //$.notify(response.message, "error");
                Swal.fire("Save Status!", response.message, "error");
                $('#CustomerName').focus();
            }
        }
    });
}

function showcustomerentry(cusname) {
    debugger;
    $.ajax({
        type: 'POST',
        url: '/ProjectMaster/ShowCustomerEntry',
        datatype: "html",
        data: {
            FieldName: 's', Cusname: cusname
            
        },
        success: function (data) {
            $("#customerContainer").html(data);
            $('#customerpopup').modal('show');
            $('#spancusterror').html("");
           
            setTimeout(function () {
                $('#CustomerName').focus();
            }, 200)

        }
    });
}
