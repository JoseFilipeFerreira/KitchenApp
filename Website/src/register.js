import React, { Component } from "react";
import { Link } from "react-router-dom";
import axios from "axios";
import "./style.css";



class Register extends Component {
  constructor(props) {
    super(props);

    this.state = {
      email: '',
      name: '',
      phone: '',
      password: '',
      birthday: ''
    };
  }


  changeHandler = (e) => {
    this.setState({ [e.target.name]: e.target.value })
  }

  validateEmail = (e) => {
    if (/^\w+([.-]?\w+)*@\w+([.-]?\w+)*(\.\w{2,3})+$/.test(e) && e !== "") {
      return true;
    }
    if (e === '') {
      alert("Empty email address");
    } else {
      alert("Invalid Email");
    }
    document.registerForm.email.focus();
    return false;
  };

  validateBirthday = (e) => {
    if (/^([12]\d{3}\/(0[1-9]|1[0-2])\/(0[1-9]|[12]\d|3[01]))$/.test(e) && e !== "") {
      return true;
    }
    if (e === '') {
      alert("Empty birthday date");
    } else {
      alert("Invalid birthday (YYYY/MM/DD)");
    }
    document.registerForm.birthday.focus();
    return false;
  };

  validateName = (e) => {
    if (/^\w[a-zA-Z ]+$/.test(e) && e !== "") {
      return true;
    }
    if (e === '') {
      alert("Empty name");
    } else {
      alert("You have entered an invalid name!");
    }
    document.registerForm.name.focus();
    return false;
  };

  validatePhone = (e) => {
    if (/^9\d{8}$/.test(e) && e !== "") {
      return true;
    }
    if (e === '') {
      alert("Empty phone number");
    } else {
      alert("You have entered an invalid phone number!");
    }
    document.registerForm.phone.focus();
    return false;
  };

  validatePassword = (e) => {
    if (8 <= e.length && e.length <= 16 && e !== '') {
      return true;
    }
    if (e === '') {
      alert("Empty password");
    } else {
      alert("Invalid password");
    }
    document.registerForm.password.focus();

    return false;
  };

  submitHandler = e => {
    e.preventDefault();
    console.log(this.state);
    
    const form = new FormData();

    let username = this.state.email;
    let name = this.state.name;
    let passwd = this.state.password;
    let phone = this.state.phone;
    let birthdate = this.state.birthday;
    

    if (this.validateEmail(username) && this.validatePassword(passwd) && this.validatePhone(phone) && this.validateBirthday(birthdate) && this.validateName(name)) {
      form.append("email", username);
      form.append("passwd", passwd);
      form.append('name', name);
      form.append('phone_number', phone);
      form.append('birthdate', birthdate);
      
      axios.post("http://localhost:1331/signup", form, {
        headers: { "Content-Type": "multipart/form-data" }, withCredentials: true,
      })
      .then(response => {
        this.props.history.push('/');
        alert('Registo efetuado!')
      })
      .catch(error => {
        console.log(error)
      });
    } else {
      document.registerForm.email.value = "";
      document.registerForm.name.value = ""
      document.registerForm.password.value = ""
      document.registerForm.phone.value = ""
      document.registerForm.birthday.value = ""
    }
  }

  render() {
    const { email, name, phone, password, birthday } = this.state;
    return (
      <form id='registerForm' name="registerForm" className="login-box" onSubmit={this.submitHandler}>
        <div className="logo">Kitchen App</div>
        <div className="login-text">Email</div>
        <input
          className="field"
          type="text"
          name="email"
          value={email}
          onChange={this.changeHandler}
        ></input>
        <div className="login-text">Nome</div>
        <input 
          className="field" 
          type="text" 
          name="name" 
          value={name} 
          onChange={this.changeHandler}
        ></input>
        <div className="login-text">Telemóvel</div>
        <input
          className="field"
          type="text"
          name="phone"
          value={phone}
          onChange={this.changeHandler}
        ></input>
        <div className="login-text">Data de Nascimento</div>
        <input
          className="field"
          type="text"
          name="birthday"
          value={birthday}
          placeholder="YYYY/MM/DD"
          onChange={this.changeHandler}
        ></input>
        <div className="login-text">Password</div>
        <input
          className="field"
          type="password"
          name="password"
          value={password}
          onChange={this.changeHandler}
        ></input>
        <div className="buttons">
          <Link to="/">
            <input className="login-button" type="button" value="Back"></input>
          </Link>
          <input
            className="register-button "
            type="submit"
            value="Register"
          ></input>
        </div>
      </form>
    );
  }
}

export default Register;
