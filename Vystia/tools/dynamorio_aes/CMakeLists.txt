cmake_minimum_required(VERSION 3.7)
project(aes_key_extractor)

# Find DynamoRIO
find_package(DynamoRIO REQUIRED)

# Create the client library
add_library(aes_key_extractor SHARED aes_key_extractor.cpp)

# Configure as DynamoRIO client
configure_DynamoRIO_client(aes_key_extractor)

# Link with DynamoRIO extensions
use_DynamoRIO_extension(aes_key_extractor drmgr)
use_DynamoRIO_extension(aes_key_extractor drreg)
