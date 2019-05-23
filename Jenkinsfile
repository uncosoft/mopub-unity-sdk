#!/usr/bin/env groovy
pipeline {
    agent any
    environment {
        ANDROID_HOME = '/Users/jenkins/Library/Android/sdk'
        UNITY_BIN = '/Applications/Unity/Unity.app/Contents/MacOS/Unity'
    }
    stages {
        stage('Build') {
            steps {
                sh './scripts/build.sh'
                sh './scripts/run_unit_tests.sh'
            }
        }
    }
    post {
        success {
            slackSend color: 'GREEN', message: "<${env.BUILD_URL}|${env.JOB_NAME} #${env.BUILD_NUMBER}> has succeeded."
        }
        failure {
            slackSend color: 'RED', message: "Attention @here <${env.BUILD_URL}|${env.JOB_NAME} #${env.BUILD_NUMBER}> has failed."
        }
    }
}
