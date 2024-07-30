function CheckLogin() {
    var email = document.getElementById("EmailLogin").value;
    var pass = document.getElementById("PasswordLogin").value;

    $.ajax({
        type: 'POST',
        url: `/CheckLogin/${email}/${pass}`,
        contentType: 'text/plain',
        crossDomain: false,
        async: true,
        success: function (response) {
            if (response=="Success"){
                window.location.href = '/';
            }
            else{
                alert("Email or Password doesn't match, Please try again !");
            }

        }

    });
}
function SignUp(){
    alert("User created successfully!");
    signInBtn.click();
}

const signInBtn = document.getElementById("signIn");
const signUpBtn = document.getElementById("signUp");

const container = document.querySelector(".container");

signInBtn.addEventListener("click", () => {
    container.classList.remove("right-panel-active");
});

signUpBtn.addEventListener("click", () => {
    container.classList.add("right-panel-active");
});

