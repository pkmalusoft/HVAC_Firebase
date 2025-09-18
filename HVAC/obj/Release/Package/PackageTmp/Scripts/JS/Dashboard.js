﻿function chart1() {
    $('#achart2').removeClass('active');
    $('#achart1').addClass('active');
    var arr1 = $('#hdnCountSeriesA').val().split(',');
    var arr2 = $('#hdnCountSeriesB').val().split(',');
    var arr3 = $('#hdnCountSeriesC').val().split(',');
    var options = {
        chart: { height: 360, type: "bar", stacked: !0, toolbar: { show: !1 }, zoom: { enabled: !0 } },
        plotOptions: { bar: { horizontal: !1, columnWidth: "15%", endingShape: "rounded" } },
        dataLabels: { enabled: !1 },
        series: [
            { name: "Air", data: arr1 },
            { name: "Sea", data: arr2 },
            { name: "Land", data: arr3 },
        ],
        xaxis: { categories: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"] },
        colors: ["#556ee6", "#f1b44c", "#34c38f"],
        legend: { position: "bottom" },
        fill: { opacity: 1 },
    },
        chart = new ApexCharts(document.querySelector("#stacked-column-chart"), options);
    chart.render();
    //options = {
    //    chart: { height: 200, type: "radialBar", offsetY: -10 },
    //    plotOptions: {
    //        radialBar: {
    //            startAngle: -135,
    //            endAngle: 135,
    //            dataLabels: {
    //                name: { fontSize: "13px", color: void 0, offsetY: 60 },
    //                value: {
    //                    offsetY: 22,
    //                    fontSize: "16px",
    //                    color: void 0,
    //                    formatter: function (e) {
    //                        return e + "%";
    //                    },
    //                },
    //            },
    //        },
    //    },
    //    colors: ["#556ee6"],
    //    fill: { type: "gradient", gradient: { shade: "dark", shadeIntensity: 0.15, inverseColors: !1, opacityFrom: 1, opacityTo: 1, stops: [0, 50, 65, 91] } },
    //    stroke: { dashArray: 4 },
    //    series: [67],
    //    labels: ["Series A"],
    //};
    //(chart = new ApexCharts(document.querySelector("#radialBar-chart"), options)).render();

}
 
function chart2() {
    $('#achart1').removeClass('active');
    $('#achart2').addClass('active');
    var arr1 = [44, 55, 41, 67, 22, 43, 36, 52, 24, 18, 36, 48];
    var arr2 = [13, 23, 20, 8, 13, 27, 18, 22, 10, 16, 24, 22];
    var arr3 = [11, 17, 15, 15, 21, 14, 11, 18, 17, 12, 20, 18];
    var options = {
        chart: { height: 360, type: "bar", stacked: !0, toolbar: { show: !1 }, zoom: { enabled: !0 } },
        plotOptions: { bar: { horizontal: !1, columnWidth: "15%", endingShape: "rounded" } },
        dataLabels: { enabled: !1 },
        series: [
            { name: "Air", data: arr1 },
            { name: "Sea", data: arr2 },
            { name: "Land", data: arr3 },
        ],
        xaxis: { categories: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"] },
        colors: ["#556ee6", "#f1b44c", "#34c38f"],
        legend: { position: "bottom" },
        fill: { opacity: 1 },
    },
        chart = new ApexCharts(document.querySelector("#stacked-column-chart"), options);
    chart.render();
    //options = {
    //    chart: { height: 200, type: "radialBar", offsetY: -10 },
    //    plotOptions: {
    //        radialBar: {
    //            startAngle: -135,
    //            endAngle: 135,
    //            dataLabels: {
    //                name: { fontSize: "13px", color: void 0, offsetY: 60 },
    //                value: {
    //                    offsetY: 22,
    //                    fontSize: "16px",
    //                    color: void 0,
    //                    formatter: function (e) {
    //                        return e + "%";
    //                    },
    //                },
    //            },
    //        },
    //    },
    //    colors: ["#556ee6"],
    //    fill: { type: "gradient", gradient: { shade: "dark", shadeIntensity: 0.15, inverseColors: !1, opacityFrom: 1, opacityTo: 1, stops: [0, 50, 65, 91] } },
    //    stroke: { dashArray: 4 },
    //    series: [67],
    //    labels: ["Series A"],
    //};
    //(chart = new ApexCharts(document.querySelector("#radialBar-chart"), options)).render();

}

function DashboardProcess() {
    $.ajax({
        type: "POST",
        url: '/Dashboard/DashboardReprocess',
        datatype: "json",        
        success: function (response) {
            if (response.status == "OK") {
                Swal.fire("Save Status!", response.message, "success");
                setTimeout(function () {
                    window.location.reload();
                }, 500)
            }
            else {
                Swal.fire("Save Status!", response.message, "error");
            }

        }
    });
}




(function ($) {

    'use strict';
    function initformControl() {

        
        chart1();
         


    }

    function init() {
        initformControl();
    }
    $(document).ready(function () {
        init();


    })


})(jQuery)
