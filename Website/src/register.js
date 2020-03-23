import React from "react";
import { Link } from "react-router-dom";
import "./style.css";

export const Register = () => (
  <div class="login-box">
    <div class="logo">Kitchen App</div>
    <div class="login-text">Email</div>
    <input class="field" type="text" name="" id=""></input>
    <div class="login-text">Nome</div>
    <input class="field" type="text" name="" id=""></input>
    <div class="login-text">Telemóvel</div>
    <input class="field" type="text" name="" id=""></input>
    <div class="login-text">Password</div>
    <input class="field" type="password" name="" id=""></input>
    <div class="buttons">
    <Link to="/">
    <input
        class="login-button"
        type="button"
        value="Back"
      ></input>
    </Link>
      <input class="register-button " type="submit" value="Register"></input>
    </div>
  </div>
);
