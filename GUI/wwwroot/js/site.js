// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
const API_URL = "https://localhost:44383";
const HOST_URL = "https://localhost:5011";

const PRODUCT_FILTER_API = `${API_URL}/api/GetListProduct/Process`;
const PRODUCT_SELECT_API = `${API_URL}/api/`;

const ORDER_SUBMIT_API = `${API_URL}/api/orders/create`;