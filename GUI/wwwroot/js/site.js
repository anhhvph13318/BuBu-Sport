// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
const API_URL = "http://localhost:5059";
const HOST_URL = "http://localhost:5011"

const PRODUCT_FILTER_API = `${API_URL}/api/GetListProduct/Process`;

const ORDER_SUBMIT_API = `${API_URL}/api/orders/create`;