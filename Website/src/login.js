import React from "react";
import { Link } from "react-router-dom";
import "./style.css";

export const Login = () => (
  <form class="login-box">
    <div class="logo">Kitchen App</div>
    <div class="login-text">Username</div>
    <input class="field" type="text" name="" id=""></input>
    <div class="login-text">Password</div>
    <input class="field" type="password" name="" id=""></input>
    <div class="buttons">
      <input class="login-button" type="submit" value="Login"></input>
      <Link to="/register">
        <input
          class="register-button"
          type="button"
          value="Register"
        ></input>
      </Link>
    </div>
  </form>
);
