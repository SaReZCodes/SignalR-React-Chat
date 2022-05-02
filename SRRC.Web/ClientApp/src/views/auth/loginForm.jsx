import Joi from "@hapi/joi";
import React from "react";
import { Redirect } from "react-router-dom";
import AppAnonymousHeader from "../../components/layout/AppAnonymousHeader";

import * as auth from "../../services/authService";
import Form from "../base/form";

class LoginForm extends Form {
    state = {
        data: { username: "", password: "" },
        errors: {}
    };

    schema = {
        username: Joi.string()
            .required()
            .label("Username"),
        password: Joi.string()
            .required()
            .label("Password")
    };

    doSubmit = async () => {
        try {
            const { data } = this.state;
            await auth.login(data.username, data.password);

            const { state } = this.props.location;
            window.location = state ? state.from.pathname : "/";
        } catch (ex) {
            if (ex.response && ex.response.status === 400) {
                const errors = { ...this.state.errors };
                errors.username = ex.response.data;
                this.setState({ errors });
            }
        }
    };

    render() {
        if (auth.getCurrentUser()) return <Redirect to="/" />;

        return (
            <React.Fragment>
                <AppAnonymousHeader />
                <div className="card">
                    <div className="card-header">
                        Login
                    </div>
                    <div className="card-body">
                        <form onSubmit={this.handleSubmit}>
                            {this.renderInput("username", "Username")}
                            {this.renderInput("password", "Password", "password")}
                            {this.renderButton("Login")}
                        </form>
                    </div>
                </div>
            </React.Fragment>
        );
    }
}

export default LoginForm;
