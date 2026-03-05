-- Database Schema for HR Cricket
CREATE DATABASE IF NOT EXISTS hr_cricket;
USE hr_cricket;

-- Blogs Table
CREATE TABLE IF NOT EXISTS blogs (
    id INT AUTO_INCREMENT PRIMARY KEY,
    title VARCHAR(255) NOT NULL,
    category VARCHAR(50),
    excerpt TEXT,
    content TEXT,
    author VARCHAR(100),
    image_url VARCHAR(255),
    reading_time INT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- Players Table
CREATE TABLE IF NOT EXISTS players (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    team VARCHAR(100),
    role VARCHAR(50),
    image_url VARCHAR(255),
    matches INT,
    innings INT,
    total_runs INT,
    average DECIMAL(5,2),
    strike_rate DECIMAL(5,2),
    hundreds INT,
    fifties INT,
    high_score INT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Matches Table (Live/Recent)
CREATE TABLE IF NOT EXISTS matches (
    id INT AUTO_INCREMENT PRIMARY KEY,
    series_name VARCHAR(255),
    team_a VARCHAR(100),
    team_b VARCHAR(100),
    score_a VARCHAR(50),
    score_b VARCHAR(50),
    status VARCHAR(100),
    match_type VARCHAR(50), -- Test, ODI, T20
    is_live BOOLEAN DEFAULT FALSE,
    match_date DATE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Schedule Table
CREATE TABLE IF NOT EXISTS schedule (
    id INT AUTO_INCREMENT PRIMARY KEY,
    match_name VARCHAR(255),
    series VARCHAR(255),
    venue VARCHAR(255),
    match_time DATETIME,
    category VARCHAR(50), -- International, T20 League, etc.
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
