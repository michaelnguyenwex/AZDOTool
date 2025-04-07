# AZDO Tool

This tool is used to update Azure DevOps test cases with automated test details.

## Usage


## Parameters

### Required Parameters

- `--testCaseId <id>`: The ID of the test case to update.
- `--automatedTestName <name>`: The name of the automated test.
- `--pat <token>`: The Personal Access Token (PAT) for authentication.

### Optional Parameters

- `--automatedTestStorage <storage>`: The storage location of the automated test. Default value: `Evolution1.OnDemand.QA.ConsumerInvestmentPortal.AcceptanceTests.dll`.
- `--organization <org>`: The Azure DevOps organization. Default value: `WexHealthTech`.
- `--project <project>`: The Azure DevOps project. Default value: `QA`.

## Example

AzdoTestCaseUpdater.exe --testCaseId 488728 --automatedTestStorage Test.dll --automatedTestName Test.test12 --organization WexHealthTech --project QA --pat 123key


## Description

This tool updates the specified Azure DevOps test case with the provided automated test details. It uses the Azure DevOps REST API to perform the update. The required parameters are `--testCaseId` and `--automatedTestName`. Optional parameters include `--automatedTestStorage`, `--organization`, `--project`, and `--pat`.

## Error Handling

If any required parameter is missing or an unknown argument is provided, the tool will display an error message and usage instructions.

## Notes

- Ensure that the Personal Access Token (PAT) is valid and has the necessary permissions to update test cases in Azure DevOps.
- The default values for optional parameters can be overridden by providing the corresponding command-line arguments.

