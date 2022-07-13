using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.TestTemplate
{
    public class TestTemplateHandler : IRequestHandler<TestTemplateRequest, TestTemplateResponse>
    {
        public TestTemplateHandler()
        {

        }
        public Task<TestTemplateResponse> Handle(TestTemplateRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
