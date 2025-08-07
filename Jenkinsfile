pipeline {
    agent any

    environment {
        DOTNET_CLI_TELEMETRY_OPTOUT = '1'
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Prepare Script') {
            steps {
                sh 'chmod +x deb-publish.sh'
            }
        }

        stage('Build & Package') {
            steps {
                sh './deb-publish.sh'
            }
        }

        stage('Archive .deb Package') {
            steps {
                archiveArtifacts artifacts: 'release/*.deb', onlyIfSuccessful: true
            }
        }
    }
}