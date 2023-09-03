﻿angular.module("dentalApp")
    .controller("LoginController", [
        "$rootScope", "$scope", "$state", "AuthService", "AppService", "toaster",
        function ($rootScope, $scope, $state, authService, appService, toaster) {
            "use strict";            
           
            $scope.credentials = { Username: "", Password: "", grant_type: "password" };

            $scope.login = function() {

                var changeRoute = function(isLoggedIn) {
                    if (isLoggedIn) {
                        var routeResponse = appService.nextRoute();

                        $scope.isLoggedIn = routeResponse.IsLoggedIn;
                        $rootScope.$broadcast(routeResponse.Broadcast);
                        $state.go(routeResponse.ToState);

                    } else {
                        $state.go("root.login");
                    }
                };


                var successCallback = function(response) {
                    console.log(response);

                    toaster.pop("success", "Successfully sign in...")

                    var success = function(response) {
                        console.log(response);
                        changeRoute(true);
                    };
                    var error = function(error) {
                        console.log(error);
                        changeRoute(false);
                    };
                    authService.userProfile().then(success, error);
                };
                var errorCallback = function(error) {
                    console.log(error);
                    //alertService.showAlert(alertService.alertType.danger, "Login Faield! invalid USERNAME or PASSWORD detected!", true);
                    toaster.pop("error", "Failed to signin! Please tray again.")
                    changeRoute(false);
                };
                $scope.promise = authService.authenticate($scope.credentials).then(successCallback, errorCallback);

            };

        }
    ]);



angular.module("dentalApp")
    .controller("AccessDeniedController", [
        "$scope", "$state", "$rootScope", "AppService",
        function ($scope, $state, $rootScope, appService) {

            $scope.defaultRoute = function() {
                var routeResponse = appService.nextRoute();

                $scope.isLoggedIn = routeResponse.IsLoggedIn;
                $rootScope.$broadcast(routeResponse.Broadcast);
                $state.go(routeResponse.ToState);
            };
        }
    ]);



angular.module("dentalApp")
    .controller("ProfileController", [
        "$scope", "UrlService", "LocalDataStorageService", "$rootScope", "$state", "HttpService", "AuthService", "toaster",
        function ($scope, urlService, localDataStorageService, $rootScope, $state, httpService, authService, toaster) {

            var init = function () {
                $scope.model = [];
                $scope.loadProfile();

                $scope.isDemoUser = authService.isDemoUser();
            }; 

            $scope.loadProfile = function () {
                var success = function(data) {
                    $scope.model = data;
                };
                var error = function(error) {
                    console.log(error);
                };
                $scope.promise = httpService.get(urlService.ProfileUrl + "/UserProfile").then(success, error);
            };

            $scope.updateProfile = function() {
                var success = function(data) {
                    console.log(data);
                    if (data.Result.Succeeded) {
                        //alertService.showAlert(alertService.alertType.success, "Success", false);
                        toaster.pop("success", "Profile update successfully");
                    }
                    else {
                        //alertService.showAlert(alertService.alertType.danger, "Failed! Please try agian", true);
                        toaster.pop("error", "Failed to update profile! Please try again.");
                    }
                    $scope.loadProfile();
                };
                var error = function(error) {
                    console.log(error);
                    toaster.pop("error","User Profile Update Faield!");
                };
                $scope.promise = httpService.add(urlService.ProfileUrl + "/UpdateProfile", $scope.model).then(success, error);
            };

            $scope.updatePassword = function() {
                var requestModel = {
                    CurrentPassword: $scope.model.CurrentPassword,
                    NewPassword: $scope.model.NewPassword,
                    RetypePassword: $scope.model.RetypePassword
                };

                var success = function(data) {
                    console.log(data);
                    if (data) {
                        //alertService.showAlert(alertService.alertType.success, "Success", false);
                        toaster.pop("success", "Password changed successfully");

                        $scope.logout();
                    }
                    else {
                        //alertService.showAlert(alertService.alertType.danger, "Failed! Please try agian", true);
                        toaster.pop("error", "Failed to change password! Please tray again.");
                    }
                    //$scope.loadProfile();
                };
                var error = function(error) {
                    console.log(error);
                    //alertService.showAlert(alertService.alertType.danger, "Failed! Please try agian", true);
                    toaster.pop("error", "Failed to change password! Please tray again.");
                };
                $scope.promise = httpService.add(urlService.ProfileUrl + "/UpdatePassword", requestModel).then(success, error);
            };

            $scope.logout = function () {

                localDataStorageService.logout();
                $rootScope.$broadcast('loggedOut');
                $state.go("root.login", {}, { reload: true });
            }

            init();
        }
    ]);