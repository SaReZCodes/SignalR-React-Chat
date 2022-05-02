import jwtDecode from "jwt-decode";

import { adminRoleName, apiUrl } from "../config.json";
import http from "./httpService";

const apiEndpoint = apiUrl + "/account";
const tokenKey = "token";

http.setJwt(getLocalJwt());

export async function login(username, password) {
    const {
        data: { access_token }
    } = await http.post(apiEndpoint + "/login", { username, password });
    console.log("JWT", access_token);
    localStorage.setItem(tokenKey, access_token);
}

export function loginWithJwt(jwt) {
    localStorage.setItem(tokenKey, jwt);
}

export function logout() {
    localStorage.removeItem(tokenKey);
}

export function getCurrentUser() {
    try {
        const jwt = localStorage.getItem(tokenKey);
        const user = jwtDecode(jwt);
        addRoles(user);
        checkExpirationDate(user);
        console.log("currentUser", user);
        return user;
    } catch (ex) {
        console.log(ex);
        logout();
        return null;
    }
}

function checkExpirationDate(user) {
    if (!user || !user.exp) {
        throw new Error("This access token doesn't have an expiration date!");
    }

    user.expirationDateUtc = new Date(0); // The 0 sets the date to the epoch
    user.expirationDateUtc.setUTCSeconds(user.exp);

    const isAccessTokenTokenExpired =
        user.expirationDateUtc.valueOf() < new Date().valueOf();
    if (isAccessTokenTokenExpired) {
        throw new Error("This access token is expired!");
    }
}

function addRoles(user) {
    const roles =
        user["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
    if (roles) {
        if (Array.isArray(roles)) {
            user.roles = roles.map(role => role.toLowerCase());
        } else {
            user.roles = [roles.toLowerCase()];
        }
    }
}

export function isAuthUserInRoles(user, requiredRoles) {
    if (!user || !user.roles) {
        return false;
    }

    if (user.roles.indexOf(adminRoleName.toLowerCase()) >= 0) {
        return true; // The `Admin` role has full access to every pages.
    }

    return requiredRoles.some(requiredRole => {
        if (user.roles) {
            return user.roles.indexOf(requiredRole.toLowerCase()) >= 0;
        } else {
            return false;
        }
    });
}

export function isAuthUserInRole(user, requiredRole) {
    return isAuthUserInRoles(user, [requiredRole]);
}

export function getLocalJwt() {
    return localStorage.getItem(tokenKey);
}