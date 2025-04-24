module.exports = async ({ github, context }) => {
  const pr = context.payload.pull_request;
  
  if (!pr) {
    console.log('No pull request found in context');
    return;
  }
  
  // Get the PR details
  console.log(`Processing PR #${pr.number}: ${pr.title}`);
  
  // Update project cards if applicable
  // This is a placeholder - you would need to implement project integration based on your specific setup
  
  // Log the action taken
  let action = context.payload.action;
  console.log(`Action taken on PR: ${action}`);
  
  // Extract task number from PR title (assuming format [TS-X] or TS-X)
  const taskMatch = pr.title.match(/\[?TS-(\d+)\]?/);
  const taskNumber = taskMatch ? taskMatch[1] : null;
  
  if (taskNumber) {
    console.log(`Associated task number: TS-${taskNumber}`);
    
    // Find the related issue
    const issues = await github.rest.issues.listForRepo({
      owner: context.repo.owner,
      repo: context.repo.repo,
      state: 'open',
      labels: 'task'
    });
    
    const relatedIssue = issues.data.find(issue => {
      return issue.title.includes(`TS-${taskNumber}`) || 
             issue.body.includes(`TS-${taskNumber}`);
    });
    
    if (relatedIssue) {
      console.log(`Found related issue: #${relatedIssue.number}`);
      
      // Link the PR to the issue
      await github.rest.issues.createComment({
        owner: context.repo.owner,
        repo: context.repo.repo,
        issue_number: relatedIssue.number,
        body: `This issue is being addressed in PR #${pr.number}`
      });
      
      // If PR is closed and merged, we can close the issue or move it to Done
      if (action === 'closed' && pr.merged) {
        await github.rest.issues.update({
          owner: context.repo.owner,
          repo: context.repo.repo,
          issue_number: relatedIssue.number,
          state: 'closed',
          state_reason: 'completed'
        });
        
        console.log(`Closed issue #${relatedIssue.number} as PR was merged`);
      }
    } else {
      console.log(`No related issue found for TS-${taskNumber}`);
    }
  } else {
    console.log('No task number found in PR title');
  }
  
  // Perform different actions based on the event type
  switch(action) {
    case 'opened':
      console.log('New PR created');
      
      // Add a comment to the PR with a checklist
      await github.rest.issues.createComment({
        owner: context.repo.owner,
        repo: context.repo.repo,
        issue_number: pr.number,
        body: `## PR Checklist
- [ ] Code follows project style guidelines
- [ ] Unit tests pass locally
- [ ] Documentation has been updated (if needed)
- [ ] UI changes have been tested on target devices (if applicable)`
      });
      break;
      
    case 'closed':
      if (pr.merged) {
        console.log('PR was merged');
      } else {
        console.log('PR was closed without merging');
      }
      break;
      
    case 'reopened':
      console.log('PR reopened');
      break;
      
    default:
      console.log(`Unhandled action: ${action}`);
  }
}; 