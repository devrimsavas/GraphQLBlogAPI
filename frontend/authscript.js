document.getElementById("loginform").addEventListener("submit",async (e)=> {
    e.preventDefault()
    try {
        const userEmail=document.getElementById("useremail").value;
        const password=document.getElementById("current-password").value;
        const BASE_URL="https://localhost:7271/graphql";
        const query=`
        mutation {
        login(loginInput: {
        email: "${userEmail}",
        password:"${password}"
        })
        {
        token 
        userId
        userEmail
        userName
        userRole
        }
        }
        `;

        const response=await fetch(BASE_URL, {
            method:"POST",
            headers: {
                "Content-Type":"application/json"
            },
            body: JSON.stringify({query:query})
        })
        const data=await response.json();
        console.log(data)
        if (data.errors && data.errors.length>0) {
            alert(data.errors[0].message);
            return;
        }
        if (data.data && data.data.login) {
            alert(`Login Succesful ${data.data.login.userEmail}`)
            document.getElementById("userIndicator").innerText=data.data.login.userName;
        }
        //token
        const token=data.data.login.token;        
        let user=data.data.login.userName
       

        localStorage.setItem("token",token)
        localStorage.setItem("user",user || "Guest")
        const currentUser = localStorage.getItem("user");
        const indicator = document.getElementById("userIndicator");

        if (indicator) {
            
            indicator.innerText = (currentUser && currentUser !== "undefined") ? currentUser : "Guest";
        }
        

        
    }catch(err) {
        console.error("error",err.message);
        alert(err.message);
    }

})




        