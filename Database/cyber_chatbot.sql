-- =============================================================================
-- Cybersecurity Awareness Chatbot - Database Script (PROG6221 POE, Part 3)
-- Student: Joshua Marc Lourens (ST10441951)
--
-- This script creates the MySQL database and the table used by the Task Assistant.
-- The application also creates these automatically on first run, but this script is
-- provided so the schema can be set up (and inspected) independently.
--
-- How to run:
--   * MySQL Workbench: File > Open SQL Script... > select this file > Execute (lightning bolt)
--   * Command line:    mysql -u root -p < cyber_chatbot.sql
-- =============================================================================

-- 1. Create the database (safe to run repeatedly) and select it.
CREATE DATABASE IF NOT EXISTS cyber_chatbot;
USE cyber_chatbot;

-- 2. Create the tasks table.
--    (Uncomment the next line if you want a clean rebuild that clears existing data.)
-- DROP TABLE IF EXISTS tasks;

CREATE TABLE IF NOT EXISTS tasks (
    Id           INT AUTO_INCREMENT PRIMARY KEY,   -- unique task id
    Title        VARCHAR(255) NOT NULL,            -- short task name
    Description  TEXT NULL,                        -- friendly explanation
    ReminderDate DATETIME NULL,                    -- optional reminder (NULL = none)
    IsCompleted  TINYINT(1) NOT NULL DEFAULT 0,    -- 0 = pending, 1 = completed
    CreatedAt    DATETIME NOT NULL                 -- when the task was created
);

-- 3. (Optional) Sample data so the table is not empty when first viewed.
--    The app works fine without these; remove this section if you prefer an empty table.
INSERT INTO tasks (Title, Description, ReminderDate, IsCompleted, CreatedAt) VALUES
('Review privacy settings',
 'Review your account privacy settings to ensure your personal data is protected.',
 DATE_ADD(NOW(), INTERVAL 3 DAY), 0, NOW()),
('Enable two-factor authentication',
 'Enable two-factor authentication to add a second layer of protection to your account.',
 NULL, 0, NOW()),
('Update my password',
 'Update to a strong, unique passphrase to keep your account secure.',
 DATE_ADD(NOW(), INTERVAL 1 DAY), 1, NOW());

-- 4. Quick check.
SELECT Id, Title, ReminderDate, IsCompleted, CreatedAt FROM tasks;
