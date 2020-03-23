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
      password: ''
    };
  }


  changeHandler = (e) => {
    this.setState({ [e.target.name]: e.target.value })
  }

  submitHandler = e => {
    e.preventDefault();
    console.log(this.state);
    
    const form = new FormData();

    let username = this.state.email;
    let name = this.state.name;
    let passwd = this.state.password;
    let phone = this.state.phone;
    
    form.append('username', username);
    form.append('name', name);
    form.append('passwd', passwd);
    form.append('phone', phone);

    axios.post('https://jsonplaceholder.typicode.com/posts', form, {
      headers: { 'Content-Type': 'multipart/form-data' },
    })
      .then(response => {
        console.log(response)
      })
      .catch(error => {
        console.log(error)
      });
  }

  render() {
    const { email, name, phone, password } = this.state;
    return (
      <form className="login-box" onSubmit={this.submitHandler}>
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