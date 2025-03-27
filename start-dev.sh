#!/bin/bash
cd TourismAPI || exit 1
dotnet run &
API_PID=$!

cd ../TourismFrontend || exit 1
dotnet run &
FRONTEND_PID=$!

cleanup() {
    echo "Завершение процессов..."
    kill $API_PID
    kill $FRONTEND_PID
    exit 0
}

trap cleanup SIGINT

wait