module.exports = async ({ github, context }) => {
  const issue = context.payload.issue;
  
  if (!issue) {
    console.log('No issue found in context');
    return;
  }
  
  // Get the issue details
  console.log(`Processing issue #${issue.number}: ${issue.title}`);
  
  // Update project cards if applicable
  // This is a placeholder - you would need to implement project integration based on your specific setup
  
  // Log the action taken
  let action = context.payload.action;
  console.log(`Action taken on issue: ${action}`);
  
  // Perform different actions based on the event type
  switch(action) {
    case 'opened':
      console.log('New issue created');
      // Add any automated labels or assignees here
      break;
      
    case 'closed':
      console.log('Issue closed');
      // Update status in project board or other tracking systems
      break;
      
    case 'reopened':
      console.log('Issue reopened');
      // Revert status changes
      break;
      
    case 'labeled':
    case 'unlabeled':
      console.log(`Issue ${action} with: ${context.payload.label.name}`);
      // Take actions based on label changes
      break;
      
    case 'assigned':
    case 'unassigned':
      console.log(`Issue ${action} to: ${context.payload.assignee?.login || 'unknown'}`);
      // Take actions based on assignment changes
      break;
      
    default:
      console.log(`Unhandled action: ${action}`);
  }
  
  // Add a comment to the issue for certain actions (optional)
  if (['opened', 'reopened'].includes(action)) {
    await github.rest.issues.createComment({
      owner: context.repo.owner,
      repo: context.repo.repo,
      issue_number: issue.number,
      body: `Thank you for this issue! It has been logged in our tracking system.`
    });
  }
}; 